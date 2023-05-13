using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.User;
using Data.ViewModels.Account;

namespace Service.IServices
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
        Task<bool> RevokeToken(string token, string ipAddress);
        Task Register(User model, string password, string origin);
        Task VerifyEmail(string token);
        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task ValidateResetToken(ValidateResetTokenRequest model);
        Task ResetPassword(ResetPasswordRequest model);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(string id);
        Task<User> Create(User account, string password);
        Task<User> Update(string id, UpdateRequest model, string userId);
        Task<User> UpdateSupervisor(string id, UpdateRequest model, string userId);
        Task Delete(string id);
        Task Activate(string id);
    }
}