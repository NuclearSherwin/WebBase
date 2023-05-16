using AutoMapper;
using Data.Entities.BaseEntity;
using Data.Entities.User;
using Data.ViewModels.Account;

namespace WebBase.Configurations
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                // User
                config.CreateMap<CreateRequest, User>().ReverseMap();
                
                config.CreateMap<RegisterRequest, User>().ReverseMap();
                
                config.CreateMap<User, AccountResponse>().ReverseMap();

                config.CreateMap<User, AuthenticateResponse>().ReverseMap();

                config.CreateMap<RegisterRequest, User>().ReverseMap();
                
                config.CreateMap<UpdateRequest, BaseUser>()
                    .ForAllMembers(x => x.Condition(
                        (src, dest, prop) =>
                        {
                            // ignore null & empty string properties
                            if (prop == null) return false;
                            if (prop is string && string.IsNullOrEmpty((string)prop)) return false;

                            // ignore null role
                            if (x.DestinationMember.Name == "Role") return false;

                            return true;
                        }
                    ));

            });
            
            return mappingConfig;
        }
    }
}