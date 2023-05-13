using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Utility;
using Data.Context;
using Data.Entities.BaseEntity;
using Data.IRepository.IBaseRepository;
using Data.ViewModels.Account;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Repository.BaseRepository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
	{
		protected readonly IMongoDatabase Database;
		private const int SingleBatchOperationLimitSize = 5000;
		private readonly FindOneAndUpdateOptions<TEntity> _updateOptions = new FindOneAndUpdateOptions<TEntity>
		{
			ReturnDocument = ReturnDocument.After
		};

		public BaseRepository(IMongoContext context)
		{
			var entityType = typeof(TEntity);
			Database = context.Database;
			Collection = Database.GetCollection<TEntity>(entityType.Name);
		}

		public IMongoCollection<TEntity> Collection { get; }
		/// <summary>
		/// The queryable of default collection
		/// </summary>
		/// <returns></returns>
		public IQueryable<TEntity> Queryable => Collection.AsQueryable();

		public async Task BulkWriteAsync(IList<WriteModel<TEntity>> models, bool isOrdered = false)
		{
			if (!models.Any())
				return;

			foreach (var batch in models.ChunkBy(5000))
			{
				await Collection.BulkWriteAsync(batch, new BulkWriteOptions { IsOrdered = isOrdered });
			}
		}

		public Task<long> CountAsync(Expression<Func<TEntity, bool>> filter)
		{
			return Collection.CountDocumentsAsync(filter);
		}

		public Task DeleteAsync(string id)
		{
			return Collection.FindOneAndDeleteAsync(_ => _.Id == id);
		}

		public Task DeleteAsync(TEntity entity)
		{
			return DeleteAsync(entity.Id);
		}

		/// <summary>
		/// Deletes multiple documents
		/// </summary>
		public Task DeleteManyAsync(Expression<Func<TEntity, bool>> filter)
		{
			return Collection.DeleteManyAsync(filter);
		}

		public TEntity Find(string id)
		{
			return Queryable.FirstOrDefault(_ => _.Id == id);
		}

		public async Task<TEntity> FindAsync(string id)
		{
			var cursor = await Collection.FindAsync(_ => _.Id == id, new FindOptions<TEntity, TEntity> { Limit = 1 });

			return await cursor.FirstOrDefaultAsync();
		}

		public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter)
		{
			var cursor = await Collection.FindAsync(filter, new FindOptions<TEntity, TEntity> { Limit = 1 });

			return await cursor.FirstOrDefaultAsync();
		}
		public async Task<IList<TEntity>> FindListAsync(FilterDefinition<TEntity> filter)
		{
			var cursor = await Collection.FindAsync(filter);

			return await cursor.ToListAsync();
		}

		public async Task<IList<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter)
		{
			var cursor = await Collection.FindAsync(filter);

			return await cursor.ToListAsync();
		}

		public async Task<IList<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter, int limit)
		{
			var cursor = await Collection.FindAsync(filter, new FindOptions<TEntity, TEntity> { Limit = limit });

			return await cursor.ToListAsync();
		}

		/// <summary>
		/// CreatedAt field is always existed for this query
		/// </summary>
		public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter)
		{
			var cursor = await Collection.FindAsync(filter, new FindOptions<TEntity, TEntity> { Limit = 1, Sort = new BsonDocument().Add("CreatedAt", 1) });

			return await cursor.FirstOrDefaultAsync();
		}

		public IEnumerable<TEntity> GetAll()
		{
			return Queryable.AsEnumerable();
		}

		public TEntity Increase<TField>(string id, Expression<Func<TEntity, TField>> field, TField value)
		{
			var update = new UpdateDefinitionBuilder<TEntity>().Inc(field, value);

			return Collection.FindOneAndUpdate<TEntity>(_ => _.Id == id, update, _updateOptions);
		}

		public Task<TEntity> IncreaseAsync<TField>(string id, Expression<Func<TEntity, TField>> field,
					TField value)
		{
			var update = new UpdateDefinitionBuilder<TEntity>().Inc(field, value);

			return Collection.FindOneAndUpdateAsync<TEntity>(_ => _.Id == id, update, _updateOptions);
		}

		public Task InsertAsync(TEntity entity, bool isAutoAssignCreatedAt = true)
		{
			if (isAutoAssignCreatedAt)
			{
				entity.CreateAt = DateTime.UtcNow;
			}

			return Collection.InsertOneAsync(entity);
		}

		public async Task InsertRangeAsync(IList<TEntity> entities)
		{
			if (entities.Count == 0)
				return;

			foreach (var batch in entities.ChunkBy(SingleBatchOperationLimitSize))
			{
				await Collection.InsertManyAsync(batch, new InsertManyOptions { IsOrdered = false });
			}
		}

		/// <summary>
		/// CreatedAt field is always existed for this query
		/// </summary>
		public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> filter)
		{
			var cursor = await Collection.FindAsync(filter, new FindOptions<TEntity, TEntity> { Limit = 1, Sort = new BsonDocument().Add("CreatedAt", -1) });

			return await cursor.FirstOrDefaultAsync();
		}

		public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> filter, string fieldName)
		{
			var cursor = await Collection.FindAsync(filter, new FindOptions<TEntity, TEntity> { Limit = 1, Sort = new BsonDocument().Add(fieldName, -1) });

			return await cursor.FirstOrDefaultAsync();
		}

		public async Task<TEntity> LastOrDefaultAsync(FilterDefinition<TEntity> filter, string sortByFieldName)
		{
			var cursor = await Collection.FindAsync(filter, new FindOptions<TEntity, TEntity> { Limit = 1, Sort = new BsonDocument().Add(sortByFieldName, -1) });

			return await cursor.FirstOrDefaultAsync();
		}
		public async Task ProcessBatchesAsync(int batchSize, Func<TEntity, Task> callback)
		{
			FilterDefinition<TEntity> filter = FilterDefinition<TEntity>.Empty;
			FindOptions<TEntity> options = new FindOptions<TEntity>
			{
				BatchSize = batchSize,
				NoCursorTimeout = false
			};

			using (IAsyncCursor<TEntity> cursor = await Collection.FindAsync(filter, options))
			{
				await cursor.ForEachAsync(async (item, index) =>
				{
					await callback(item);
				});
			}
		}

		public Task UpdateAsync(TEntity entity)
		{
			entity.CreateAt = DateTime.UtcNow;
			return Collection.FindOneAndUpdateAsync(_ => _.Id == entity.Id, new ObjectUpdateDefinition<TEntity>(entity));
		}

		public void UpdateMany<TField>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TField>> field,
					TField value)
		{
			var update = new UpdateDefinitionBuilder<TEntity>().Set(field, value);

			Collection.UpdateMany(filter, update);
		}

		public Task UpdateManyAsync<TField>(Expression<Func<TEntity, bool>> filter,
					Expression<Func<TEntity, TField>> field, TField value, UpdateOptions options = null)
		{
			var update = new UpdateDefinitionBuilder<TEntity>().Set(field, value);

			return Collection.UpdateManyAsync(filter, update, options);
		}

		public Task UpdateManyAsync<TField>(Expression<Func<TEntity, bool>> filter,
					string field, TField value)
		{
			var update = new UpdateDefinitionBuilder<TEntity>().Set(field, value);

			return Collection.UpdateManyAsync(filter, update);
		}

		public Task UpdateManyAsync(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> update)
		{
			return Collection.UpdateManyAsync(filter, update);
		}

		public Task UpdateManyAsync(Expression<Func<TEntity, bool>> filter,
					List<UpdateManyEntitiesParams<TEntity, dynamic>> fields)
		{
			var updatesList = new List<UpdateDefinition<TEntity>>();
			var builder = new UpdateDefinitionBuilder<TEntity>();

			fields.ToList().ForEach(f =>
			{
				updatesList.Add(builder.Set(f.Field, f.Value));
			});

			var updates = Builders<TEntity>.Update.Combine(updatesList);

			return Collection.UpdateManyAsync(filter, updates);
		}

		public Task<TEntity> UpdateOneAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
		{
			return Collection.FindOneAndUpdateAsync(filter, update, _updateOptions);
		}

		public Task UpdateOneAsync<TField>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TField>> field, TField value, UpdateOptions options = null)
		{
			var update = new UpdateDefinitionBuilder<TEntity>().Set(field, value);

			return Collection.UpdateOneAsync(filter, update, options);
		}

		public Task<TEntity> UpdateOneAsync<TField>(string id, Expression<Func<TEntity, TField>> field,
					TField value)
		{
			var update = new UpdateDefinitionBuilder<TEntity>().Set(field, value);

			return Collection.FindOneAndUpdateAsync<TEntity>(_ => _.Id == id, update, _updateOptions);
		}

		public Task UpdateOneAsync(string id, IEnumerable<UpdateManyEntitiesParams<TEntity, dynamic>> fields)
		{
			var updatesList = new List<UpdateDefinition<TEntity>>();
			var builder = new UpdateDefinitionBuilder<TEntity>();

			fields.ToList().ForEach(f =>
			{
				updatesList.Add(builder.Set(f.Field, f.Value));
			});

			var updates = Builders<TEntity>.Update.Combine(updatesList);

			return Collection.FindOneAndUpdateAsync(_ => _.Id == id, updates);
		}

		public Task UpdateOneAsync(Expression<Func<TEntity, bool>> filter,
					IEnumerable<UpdateManyEntitiesParams<TEntity, dynamic>> fields)
		{
			var updatesList = new List<UpdateDefinition<TEntity>>();
			var builder = new UpdateDefinitionBuilder<TEntity>();

			fields.ToList().ForEach(f =>
			{
				updatesList.Add(builder.Set(f.Field, f.Value));
			});

			var updates = Builders<TEntity>.Update.Combine(updatesList);

			return Collection.UpdateOneAsync(filter, updates);
		}

		/// <summary>
		/// Note: This method impacts to database performance if there are many entities to update
		/// </summary>
		public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
		{
			foreach (var entity in entities)
			{
				await UpdateAsync(entity);
			}
		}
		
		public Task AddToSetEachAsync<TField>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, IEnumerable<TField>>> field, IEnumerable<TField> value)
		{
			var updates = Builders<TEntity>.Update.AddToSetEach(field, value);

			return Collection.UpdateManyAsync(filter, updates);
		}
	}
}