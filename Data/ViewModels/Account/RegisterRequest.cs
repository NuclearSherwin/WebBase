using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.Constants;

namespace Data.ViewModels.Account
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string FullName { get; set; }
        public StringEnums.Gender Gender { get; set; }
        [Required]
        [MinLength(3)]
        public string Field { get; set; }
        [Required]
        [MinLength(3)]
        public string SchoolYear { get; set; }
        [Required]
        [MinLength(3)]
        public string ClassName { get; set; }
        [Required]
        public string IdStudent { get; set; }
        
        public string? CitizenIdentification { get; set; }
        [Required]
        public string StudentCard { get; set; }
        [JsonIgnore]
        public string ImgPath { get; set; }
        [Required]
        [MinLength(3)]
        public string PhoneNumber { get; set; }
        public string FaceBook { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }
        public string SelfDescription { get; set; }
        public string FavoriteMaxim { get; set; }
        public string Skill { get; set; }
        public string Experience { get; set; }
        public string Advantage { get; set; }
        public string Disadvantage { get; set; }
        public string CareerOrientation { get; set; }
        public string DesireCareer { get; set; }
        
        public Dictionary<string, object>? SupervisorInfo { get; set; }


        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }
        
        [Required] 
        public StringEnums.Roles Role { get; set; }
    }
}