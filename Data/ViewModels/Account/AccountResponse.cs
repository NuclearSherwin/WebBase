using System;
using Data.Entities.User;

namespace Data.ViewModels.Account
{
    public class AccountResponse
    {
        public string FirstName { get; set; } // First name of the user
            public string LastName { get; set; } // Last name of the user
            public string Password { get; set; } // Encrypted password of the user
            public string PhoneNumber { get; set; } // Phone number of the user
            public DateTime DateOfBirth { get; set; } // Date of birth of the user
            public Address Address { get; set; } // Address of the user
            
            public string Street { get; set; } // Street name and number of the address
            public string City { get; set; } // City of the address
            public string State { get; set; } // State or province of the address
            public string ZipCode { get; set; } // Zip code or postal code of the address
            public string Country { get; set; } // Country of the address
            
            
            public bool IsVerified { get; set; }
            public bool IsDeleted { get; set; }
    }
}