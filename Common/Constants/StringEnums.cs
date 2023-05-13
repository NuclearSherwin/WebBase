using System;

namespace Common.Constants
{
    public static class StringEnums
    {
        public static string ToValue(this Enum thisEnum)
        {
            string output = null;
            var type = thisEnum.GetType();

            var fieldInfo = type.GetField(thisEnum.ToString());
            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
                if (attrs!= null && attrs.Length > 0)
                {
                    output = attrs[0].Value;
                }
            }

            return output;
        }

        public static T GetValueFromStringValue<T>(string value) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(StringValue))is StringValue attribute)
                {
                    if (attribute.Value == value)
                    {
                        return (T)field.GetValue(null);
                    }
                    else
                    {
                        if (field.Name == value)
                        {
                            return (T)field.GetValue(null);
                        }
                    }
                }
            }

            throw new ArgumentException("Not Found", value);
        }

        private class StringValue : Attribute
        {
            public StringValue(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }

        public enum Test
        {
            [StringValue("test")] Test
        }
        
        public enum AppErrorCode
        {
            Error,
            Warning,
            Info
        }
        
        public enum ErrorMessages
        {
            
        }
        
        public enum EmailExceptionType
        {
            [StringValue("Info")] Info,
            [StringValue("Warning")] Warning,
            [StringValue("Error")] Error,
        }
        
        public enum Roles
        {
            [StringValue("seniorAdmin")] SeniorAdmin,
            [StringValue("juniorAdmin")] JuniorAdmin,
            [StringValue("seniorStaff")] SeniorStaff,
            [StringValue("juniorStaff")] JuniorStaff,
            [StringValue("supervisor")] Supervisor,
            [StringValue("user")] User,
            [StringValue("noRole")] NoRole
        }
        
        // for gender when register
        public enum Gender
        {
            [StringValue("Male")] Male,
            [StringValue("Female")] Female,
            [StringValue("Other")] Other,
        }
        
        public enum VideoKind
        {
            [StringValue("ShadowShow")] ShadowShow,
            [StringValue("LightCareer")] LightCareer,
            [StringValue("WorKnowledge")] WorKnowledge
        }

        public enum FavoriteWork
        {
            [StringValue("NeededWork")] NeededWork,
            [StringValue("IncomeAndWelfare")] IncomeAndWelfare,
            [StringValue("WorkEnvironment")] WorkEnvironment,
            [StringValue("SkillsAndKnowledge")] SkillsAndKnowledge,
            [StringValue("TrainingAndInstruction")] TrainingAndInstruction,
            [StringValue("PromotionOpportunities")] PromotionOpportunities,
            [StringValue("Other")] Other
        }
        
        public enum EmailType
        {
        }
        
        public enum AppointmentStatus
        {
            [StringValue("Ready")] Ready,
            [StringValue("Sent")] Sent,
            [StringValue("Fail")] Fail,
            [StringValue("Close")] Close,
        }
        
        public enum RequestStatus
        {
            [StringValue("Pending")] Pending,
            [StringValue("Reject")] Reject,
            [StringValue("Approve")] Approve,
        }
    }
}