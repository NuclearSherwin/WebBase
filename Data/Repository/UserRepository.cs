using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.BaseEntity;
using Data.Entities.User;
using Data.IRepository.IBaseRepository;
using Data.ViewModels.Account;

namespace Data.Repository
{
    public static class UserRepository
    {
        public static Task UpdateRefreshToken(this IBaseRepository<User> repository, string id, List<RefreshToken> refreshTokens,  string updateBy)
        {
            var updates = new List<UpdateManyEntitiesParams<User, dynamic>>
            {
                new UpdateManyEntitiesParams<User, dynamic> {Field = _ => _.UpdateAt, Value = DateTime.UtcNow},
                new UpdateManyEntitiesParams<User, dynamic> {Field = _ => _.UpdateBy, Value = updateBy},
                new UpdateManyEntitiesParams<User, dynamic> {Field = _ => _.RefreshTokens, Value = refreshTokens}
            };

            return repository.UpdateOneAsync(_ => _.Id == id && !_.IsDeleted, updates);
        }
    }
}