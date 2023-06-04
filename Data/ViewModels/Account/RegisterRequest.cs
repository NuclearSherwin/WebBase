using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.Constants;
using Data.Entities.User;

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


        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }
        
        // [Required] 
        // public StringEnums.Roles Role { get; set; }
        
        public string FirstName { get; set; } // First name of the user
        
        public string LastName { get; set; } // Last name of the 

        public string PhoneNumber { get; set; } // Phone number of the user
        
        public DateTime DateOfBirth { get; set; } // Date of birth of the user
        
        public Address Address { get; set; } // Address of the user
    }
    
}