using System.ComponentModel.DataAnnotations;
using Common.Constants;

namespace Data.ViewModels.Account
{
    public class CreateRequest
    {
        private StringEnums.Roles _role;
        public StringEnums.Roles Role
        {
            get => _role;
            set => _role = StringEnums.Roles.User;
        }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}