using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels.Account
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}