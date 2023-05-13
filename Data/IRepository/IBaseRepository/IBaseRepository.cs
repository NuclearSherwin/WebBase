using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Entities.BaseEntity;
using Data.ViewModels.Account;
using MongoDB.Driver;

namespace Data.IRepository.IBaseRepository
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
	{
		IMongoCollection<TEntity> Collection { get; }

		IQueryable<TEntity> Queryable { get; }

		Task BulkWriteAsync(IList<WriteModel<TEntity>> models, bool isOrdered = false);

		Task<long> CountAsync(Expression<Func<TEntity, bool>> filter);

		Task DeleteAsync(string id);

		Task DeleteAsync(TEntity entity);

		Task DeleteManyAsync(Expression<Func<TEntity, bool>> filter);

		TEntity Find(string id);

		Task<TEntity> FindAsync(string id);
		Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter);

		Task<IList<TEntity>> FindListAsync(FilterDefinition<TEntity> filter);

		Task<IList<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter);

		Task<IList<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter, int limit);

		Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter);

		IEnumerable<TEntity> GetAll();

		TEntity Increase<TField>(string id, Expression<Func<TEntity, TField>> field, TField value);

		Task<TEntity> IncreaseAsync<TField>(string id, Expression<Func<TEntity, TField>> field, TField value);

		Task InsertAsync(TEntity entity, bool isAutoAssignCreatedAt = true);

		Task InsertRangeAsync(IList<TEntity> entities);

		Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> filter);

		Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> filter, string fieldName);

		Task<TEntity> LastOrDefaultAsync(FilterDefinition<TEntity> filter, string sortByFieldName);
		Task ProcessBatchesAsync(int batchSize, Func<TEntity, Task> callback);

		Task UpdateAsync(TEntity entity);

		void UpdateMany<TField>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TField>> field, TField value);

		Task UpdateManyAsync<TField>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TField>> field, TField value, UpdateOptions options = null);

		Task UpdateManyAsync<TField>(Expression<Func<TEntity, bool>> filter, string field, TField value);

		Task UpdateManyAsync(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> update);

		Task UpdateManyAsync(Expression<Func<TEntity, bool>> filter, List<UpdateManyEntitiesParams<TEntity, dynamic>> fields);

		Task<TEntity> UpdateOneAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update);

		Task UpdateOneAsync(string id, IEnumerable<UpdateManyEntitiesParams<TEntity, dynamic>> fields);

		Task UpdateOneAsync<TField>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TField>> field, TField value, UpdateOptions options = null);

		Task UpdateOneAsync(Expression<Func<TEntity, bool>> filter, IEnumerable<UpdateManyEntitiesParams<TEntity, dynamic>> fields);

		Task<TEntity> UpdateOneAsync<TField>(string id, Expression<Func<TEntity, TField>> field, TField value);

		Task AddToSetEachAsync<TField>(Expression<Func<TEntity, bool>> filter,
			Expression<Func<TEntity, IEnumerable<TField>>> field, IEnumerable<TField> value);
	}
}