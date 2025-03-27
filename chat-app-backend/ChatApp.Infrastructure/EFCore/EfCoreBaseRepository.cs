using ChatApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.EFCore;

/// <summary>
/// EF Core基础仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TId">实体ID类型</typeparam>
public abstract class EfCoreBaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected EfCoreBaseRepository(DbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(TId id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
        {
            return false;
        }

        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await DbSet.AddRangeAsync(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }
        await Context.SaveChangesAsync();
        return entities;
    }

    public virtual async Task<bool> DeleteRangeAsync(IEnumerable<TId> ids)
    {
        var entities = await DbSet.Where(e => ids.Contains(GetEntityId(e))).ToListAsync();
        if (!entities.Any())
        {
            return false;
        }

        DbSet.RemoveRange(entities);
        await Context.SaveChangesAsync();
        return true;
    }

    public virtual IQueryable<TEntity> GetQueryable()
    {
        return DbSet.AsQueryable();
    }

    protected abstract TId GetEntityId(TEntity entity);
} 