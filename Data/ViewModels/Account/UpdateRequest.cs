using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Constants;
using Data.Entities.User;

namespace Data.ViewModels.Account
{
    public class UpdateRequest
    {
        private string _firstName;
        private string _lastName;
        private DateTime _dateOfBirth;
        private string _password;   
        private string _confirmPassword;
        private StringEnums.Roles _role;
        private string _email;
        private string _phoneNumber;
        private Address _address;


        [MinLength(2)]
        public string FirstName
        {
            get => _firstName;
            set => _firstName = replaceEmptyWithNull(value);
        }
        
        [MinLength(3)]
        public string LastName
        {
            get => _lastName;
            set => _lastName = replaceEmptyWithNull(value);
        }
        
        
        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set => _dateOfBirth = (DateTime)replaceEmptyWithNullDateTime(value);
        }

        public StringEnums.Roles Role
        {
            get => _role;
            set
            {
                _role = value;
                _role = StringEnums.Roles.User;
            }
        }

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }
        

        [MinLength(3)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }
        

        [MinLength(3)]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = replaceEmptyWithNull(value);
        }
        
        [MinLength(3)]
        public Address Address
        {
            get => _address;
            set => _address = replaceEmptyWithNullAddress(value);
        }
        
        
        


        // helpers
        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
        
        private DateTime? replaceEmptyWithNullDateTime(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            else if (DateTime.TryParse(value.ToString(), out DateTime date))
            {
                return date;
            }
            else
            {
                throw new ArgumentException("Invalid date format", nameof(value));
            }
        }
        
        
        private Address replaceEmptyWithNullAddress(Address value)
        {
            if (value == null)
            {
                return null;
            }

            // else
            Address address = new Address
            {
                Street = replaceEmptyWithNull(value.Street),
                City = replaceEmptyWithNull(value.City),
                State = replaceEmptyWithNull(value.State),
                ZipCode = replaceEmptyWithNull(value.ZipCode),
                Country = replaceEmptyWithNull(value.Country)
            };

            return address;
        }

    }
}