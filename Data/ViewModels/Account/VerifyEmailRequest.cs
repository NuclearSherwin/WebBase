using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels.Account
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}