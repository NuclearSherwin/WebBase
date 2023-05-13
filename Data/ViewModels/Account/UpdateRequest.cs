using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Constants;

namespace Data.ViewModels.Account
{
    public class UpdateRequest
    {
        private string _password;   
        private string _confirmPassword;
        private StringEnums.Roles _role;
        private string _fullName;
        private string _field;
        private string _schoolYear;
        private string _idStudent;
        private string _citizenIdentification;
        private string _username;
        private string _email;
        private string _studentCard;
        private string _phoneNumber;
        private string _facebook;
        private string _instagram;
        private string _linkedIn;
        private string _selfDescription;
        private string _favoriteMaxim;
        private string _skill;
        private string _experience;
        private string _advantage;
        private string _disadvantage;
        private string _careerOrientation;
        private string _desireCareer;
        private Dictionary<string, object> _supervisorInfo;

        public StringEnums.Roles Role
        {
            get => _role;
            set => _role = StringEnums.Roles.User;
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
        public string Field
        {
            get => _field;
            set => _field = replaceEmptyWithNull(value);
        }
        
        public string SchoolYear
        {
            get => _schoolYear;
            set => _schoolYear = replaceEmptyWithNull(value);
        }

        public string IdStudent
        {
            get => _idStudent;
            set => _idStudent = replaceEmptyWithNull(value);
        }

        public string CitizenIdentification
        {
            get => _citizenIdentification;
            set => _citizenIdentification = replaceEmptyWithNull(value);
        }

        public string FullName
        {
            get => _fullName;
            set => _fullName = replaceEmptyWithNull(value);
        }

        [MinLength(3)]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = replaceEmptyWithNull(value);
        }
        
        
        // [Compare("Password")]
        // public string ConfirmPassword 
        // {
        //     get => _confirmPassword;
        //     set => _confirmPassword = replaceEmptyWithNull(value);
        // }


        public string StudentCard
        {
            get => _studentCard;
            set => _studentCard = replaceEmptyWithNull(value);
        }

        public string FaceBook
        {
            get => _facebook;
            set => _facebook = replaceEmptyWithNull(value);
        }

        public string Instagram
        {
            get => _instagram;
            set => _instagram = replaceEmptyWithNull(value);
        }

        public string LinkedIn
        {
            get => _linkedIn;
            set => _linkedIn = replaceEmptyWithNull(value);
        }

        public string SelfDescription
        {
            get => _selfDescription;
            set => _selfDescription = replaceEmptyWithNull(value);
        }

        public string FavoriteMaxim
        {
            get => _favoriteMaxim;
            set => _favoriteMaxim = replaceEmptyWithNull(value);
        }

        public string Skill
        {
            get => _skill;
            set => _skill = replaceEmptyWithNull(value);
        }

        public string Experience
        {
            get => _experience;
            set => _experience = replaceEmptyWithNull(value);
        }

        public string Advantage
        {
            get => _advantage;
            set => _advantage = replaceEmptyWithNull(value);
        }

        public string Disadvantage
        {
            get => _disadvantage;
            set => _disadvantage = replaceEmptyWithNull(value);
        }

        public string CareerOrientation
        {
            get => _careerOrientation;
            set => _careerOrientation = replaceEmptyWithNull(value);
        }

        public string DesireCareer
        {
            get => _desireCareer;
            set => _desireCareer = replaceEmptyWithNull(value);
        }
        
        public Dictionary<string, object> SupervisorInfo { get; set; }


        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}