using ChatApp.Domain.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ChatApp.Infrastructure.Repositories;

/// <summary>
/// MongoDB基础仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TId">实体ID类型</typeparam>
public abstract class MongoBaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : class
{
    protected readonly IMongoCollection<TEntity> Collection;

    protected MongoBaseRepository(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<TEntity>(collectionName);
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Collection.Find(_ => true).ToListAsync();
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await Collection.InsertOneAsync(entity);
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var id = GetEntityId(entity);
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        await Collection.ReplaceOneAsync(filter, entity);
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(TId id)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        var result = await Collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await Collection.InsertManyAsync(entities);
        return entities;
    }

    public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        var bulkOps = entities.Select(entity =>
            new ReplaceOneModel<TEntity>(
                Builders<TEntity>.Filter.Eq("_id", GetEntityId(entity)),
                entity)
            { IsUpsert = true });

        await Collection.BulkWriteAsync(bulkOps);
        return entities;
    }

    public virtual async Task<bool> DeleteRangeAsync(IEnumerable<TId> ids)
    {
        var filter = Builders<TEntity>.Filter.In("_id", ids);
        var result = await Collection.DeleteManyAsync(filter);
        return result.DeletedCount > 0;
    }

    public IQueryable<TEntity> GetQueryable()
    {
        throw new NotImplementedException();
    }

    protected abstract TId GetEntityId(TEntity entity);
} 