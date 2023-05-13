using System;
using System.Linq.Expressions;
using Data.Entities.BaseEntity;

namespace Data.ViewModels.Account
{
    public class UpdateManyEntitiesParams<TEntity, TField> where TEntity : BaseEntity
    {
        public Expression<Func<TEntity, TField>> Field { get; set; }
        public TField Value { get; set; }
    }
}