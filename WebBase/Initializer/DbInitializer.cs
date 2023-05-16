using System;
using System.Threading.Tasks;
using Common.Constants;
using Data.Entities.User;
using Data.IRepository.IBaseRepository;
using Service.IServices;

namespace WebBase.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IBaseRepository<User> _userRepo;
        private readonly IUserService _userService;

        public DbInitializer(IBaseRepository<User> userRepo, IUserService userService)
        {
            _userRepo = userRepo;
            _userService = userService;
        }
        
        public async Task Initialize()
        {
            var userDb = await _userRepo.FindAsync(_ => _.Email == "Admin@gmail.com" && !_.IsDeleted);
            if (userDb != null)
                return;

            var user = new User()
            {
                Email = "Admin@gmail.com",
                Role = StringEnums.Roles.SeniorAdmin,
                AcceptTerms = true,
                Verified = DateTime.Now,
            };

            await _userService.Register(user, "Paika16032002pro^^@@", String.Empty);
            await _userRepo.UpdateOneAsync(u => u.Email == "Admin@gmail.com", x => x.VerificationToken, null);
        }
    }
}