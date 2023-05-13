using System.Text.Json.Serialization;
using Common.Constants;
using Data.Entities.User;

namespace Data.ViewModels.Account
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Email = user.Email;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
            Role = user.Role;
            IsVerified = user.IsVerified;
        }
        
        public string Email { get; set; }
        public StringEnums.Roles Role { get; set; }
        public bool IsVerified { get; set; }
    }
}