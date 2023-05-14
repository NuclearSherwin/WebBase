using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Common.Constants;
using Common.Exceptions;
using Data.Entities.BaseEntity;
using Data.Entities.User;
using Data.IRepository.IBaseRepository;
using Data.Repository;
using Data.ViewModels.Account;
using Microsoft.IdentityModel.Tokens;
using Service.IServices;
using Service.Utility;

namespace Service.Services
{
    public class UserService : IUserService
    {
        // repository area
        private readonly IBaseRepository<User> _userRepo;
        // service area
        private readonly ISendMailErrorService _emailErrorService;
        private readonly ISendMailBusinessService _sendMailBusiness;

        public UserService(
                IBaseRepository<User> userRepo,
                ISendMailErrorService emailErrorService,
                ISendMailBusinessService sendMailBusiness
            )
        {
            _userRepo = userRepo;
            _emailErrorService = emailErrorService;
            _sendMailBusiness = sendMailBusiness;
        }
        
        
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = await _userRepo.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == model.Email.ToLower() && u.Verified != null && !u.IsDeleted);

            var passwordHash = Encryption.DecryptPassword(user, model.Password);
            
            // validate 
            if (user == null || passwordHash != user.PasswordHash)
                throw new Exception("Email or password is incorrect");

            if (user.Verified == null)
                throw new Exception("Email need to be verify");
            
            // authentication successful so generate JWT and Refresh tokens
            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);
            
            // save refresh token
            if (user.RefreshTokens == null)
                user.RefreshTokens = new List<RefreshToken>();
            
            user.RefreshTokens.Add(refreshToken);
            
            // remove old refresh tokens from account
            RemoveOldRefreshTokens(user);

            await _userRepo.UpdateRefreshToken(user.Id, user.RefreshTokens, refreshToken.Token);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await _userRepo.FirstOrDefaultAsync(u =>
                u.RefreshTokens.Any(t => t.Token == token) && !u.IsDeleted);
            
            // return null if no user found with tokne
            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(r => r.Token == token);
            
            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;
            
            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.Now;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            
            RemoveOldRefreshTokens(user);

            await _userRepo.UpdateRefreshToken(user.Id, user.RefreshTokens, ipAddress);
            
            // generate new jwt
            var jwtToken = GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            var user = await _userRepo.FirstOrDefaultAsync(u =>
                u.RefreshTokens.Any(t => t.Token == token) && !u.IsDeleted);
            
            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(r => r.Token == token);
            
            // return false if token is not active
            if (!refreshToken.IsActive) return false;
            
            // revoke token and save
            refreshToken.Revoked = DateTime.Now;
            refreshToken.RevokedByIp = ipAddress;

            await _userRepo.UpdateRefreshToken(user.Id, user.RefreshTokens, ipAddress);

            return true;

        }

        public async Task Register(User model, string password, string origin)
        {
            try
            {
                // validate
                var userList =
                    await _userRepo.FindAsync(u => u.Email.ToLower() == model.Email.ToLower() && !u.IsDeleted);

                if (userList != null)
                {
                    await SendAlreadyRegisterEmail(model.Email.ToLower(), origin);
                    throw new AppException("" + model.Email.ToLower() + "' is already taken'");
                }

                if (model.Email == null)
                    throw new AppException("Email is not empty!");
                if (model.FirstName == null)
                    throw new AppException("First name is not empty");
                if (model.LastName == null)
                    throw new AppException("Last name is not empty");
                if (model.PhoneNumber == null)
                    throw new AppException("Last name is not empty");
                if (model.Address == null)
                    throw new AppException("Address is not empty");
                
                
                // hash password
                Encryption.EncryptPassword(model, password);
                model.VerificationToken = RandomTokenString();
                
                // save user
                await _userRepo.InsertAsync(model);
                
                // send mail
                await SendVerificationEmail(model, origin);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task VerifyEmail(string token)
        {
            var account = await _userRepo.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (account == null) throw new AppException("Verification failed");
            
            account.Verified = DateTime.Now;
            account.VerificationToken = null;

            await _userRepo.UpdateAsync(account);
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _userRepo.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
            
            // always return ok response to prevent email enumeration
            if (account == null) return;
            
            // create reset token that expire after 1 day
            account.ResetToken = RandomTokenString();
            account.ResetTokenExpires = DateTime.Now.AddDays(1);

            await _userRepo.UpdateAsync(account);
            
            // send email
            await SendPasswordResetEmail(account, origin);

        }

        public async Task ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account =
                await _userRepo.FirstOrDefaultAsync(u =>
                    u.ResetToken == model.Token && u.ResetTokenExpires > DateTime.Now);

            if (account == null)
                throw new AppException("Invalid token");
        }

        public async Task ResetPassword(ResetPasswordRequest model)
        {
            var account = await _userRepo.FirstOrDefaultAsync(u => u.ResetToken == model.Token &&
                                                                   u.ResetTokenExpires > DateTime.Now);

            if (account == null)
                throw new AppException("Invalid token");
            
            // hash password
            Encryption.EncryptPassword(account, model.Password);
            
            // update password and remove reset token
            account.PasswordReset = DateTime.Now;
            account.ResetToken = null;
            account.ResetTokenExpires = null;
            account.UpdateAt = DateTime.Now;

            await _userRepo.UpdateAsync(account);

        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepo.FindListAsync(_ => _.IsDeleted);
        }

        public async Task<User> GetById(string id)
        {
            var result = await GetAccount(id);
            return result;
        }

        public async Task<User> Create(User account, string password)
        {
            // validate
            if ((await _userRepo.FindAsync(u => u.Email == account.Email)) != null)
                throw new AppException($"$Email '{account.Email}' is already taken");
            
            // map model to new account object
            account.CreateAt = DateTime.Now;
            account.Verified = DateTime.Now;
            
            // hash password
            Encryption.EncryptPassword(account, password);
            // save new user
            await _userRepo.InsertAsync(account);

            return account;
        }

        public async Task<User> Update(string id, UpdateRequest model, string userId)
        {
            var account = await GetAccount(id);
            
            // validate
            if (account.Email.ToLower() != model.Email.ToLower() && await _userRepo.FindAsync(x => String.Equals(x.Email, model.Email, StringComparison.CurrentCultureIgnoreCase)) != null)
                throw new AppException($"Email '{model.Email}' is already taken");
            
            // hash password if it was enter
            if (!string.IsNullOrEmpty(model.Password))
                Encryption.EncryptPassword(account, model.Password);
            
            // copy model to account and save
            account.UpdateAt = DateTime.Now;
            account.UpdateBy = userId;
            account.FirstName = model.FirstName;
            account.LastName = model.LastName;
            account.DateOfBirth = (DateTime)model.DateOfBirth;
            account.Address = model.Address;

            await _userRepo.UpdateAsync(account);

            return account;
        }

        public Task<User> UpdateSupervisor(string id, UpdateRequest model, string userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task Delete(string id)
        {
            var account = await GetAccount(id);
            account.IsDeleted = true;
            await _userRepo.UpdateAsync(account);
        }

        public async Task Activate(string id)
        {
            var account = await GetDeleteAccount(id);
            account.IsDeleted = false;
            await _userRepo.UpdateAsync(account);
        }

        
        
        
        // -------------- helper method ----------------------
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", user.Id),
                    new Claim("Role", user.Role.ToValue())
                }),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.Now.AddDays(7),
                    Created = DateTime.Now,
                    CreatedByIp = ipAddress
                };
            }
        }

        private void RemoveOldRefreshTokens(User account)
        {
            account.RefreshTokens.RemoveAll(r => !r.IsActive
                                                 && r.Created.AddDays(AppSettings.RefreshTokenTtl) <= DateTime.Now);
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private async Task SendVerificationEmail(User account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p>
                                   <a  
                                    style=""display: inline-block;
                                            padding: 10px 20px;
                                            background-color: #00cc66;
                                            color: #fff;
                                            text-decoration: none;
                                            font-weight: bold;
                                            border: none;
                                            border-radius: 5px; "" 
                                               href=""{verifyUrl}"">Click to verify
                                    </a>
                                </p>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }
            await _sendMailBusiness.SendMailAsync(
                account.Email,
                subject: "Sign-up Verification API - Verify Email",
                $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }


        private async Task<User> GetAccount(string id)
        {
            var account = await _userRepo.FindAsync(u => u.Id == id && !u.IsDeleted);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return account;
        }

        private async Task SendAlreadyRegisterEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/forgot-password</code> api route.</p>";

            await _sendMailBusiness.SendMailAsync(
                email,
                "Sign-up Verification API - Email Already Registered",
                $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private async Task SendPasswordResetEmail(User account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p>
                                <a 
                                    style=""display: inline-block;
                                            padding: 10px 20px;
                                            background-color: #00cc66;
                                            color: #fff;
                                            text-decoration: none;
                                            font-weight: bold;
                                            border: none;
                                            border-radius: 5px; "" 
                                    href=""{resetUrl}"">Reset password
                                </a>
                             </p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
            }
            
            await _sendMailBusiness.SendMailAsync(
                account.Email,
                subject: "Sign-up Verification API - Reset Password",
                $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }

        private async Task<User> GetDeleteAccount(string id)
        {
            var account = await _userRepo.FindAsync(u => u.Id == id && u.IsDeleted);
            if (account == null) throw new KeyNotFoundException("Account not found");

            return account;
        }
    }
}