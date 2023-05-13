using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Common.Constants;
using Data.Entities.User;
using Data.IRepository.IBaseRepository;
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
            var jwtToken = 
        }

        public Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RevokeToken(string token, string ipAddress)
        {
            throw new System.NotImplementedException();
        }

        public Task Register(User model, string password, string origin)
        {
            throw new System.NotImplementedException();
        }

        public Task VerifyEmail(string token)
        {
            throw new System.NotImplementedException();
        }

        public Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            throw new System.NotImplementedException();
        }

        public Task ValidateResetToken(ValidateResetTokenRequest model)
        {
            throw new System.NotImplementedException();
        }

        public Task ResetPassword(ResetPasswordRequest model)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetById(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Create(User account, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Update(string id, UpdateRequest model, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> UpdateSupervisor(string id, UpdateRequest model, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task Activate(string id)
        {
            throw new System.NotImplementedException();
        }

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
        
        
    }
}