using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels.Account
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}