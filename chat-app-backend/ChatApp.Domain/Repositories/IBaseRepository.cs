namespace ChatApp.Domain.Repositories;

/// <summary>
/// 基础仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TId">实体ID类型</typeparam>
public interface IBaseRepository<TEntity, TId> where TEntity : class
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>实体</returns>
    Task<TEntity?> GetByIdAsync(TId id);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns>实体列表</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns>添加后的实体</returns>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns>更新后的实体</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteAsync(TId id);

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns>添加后的实体列表</returns>
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns>更新后的实体列表</returns>
    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="ids">实体ID列表</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteRangeAsync(IEnumerable<TId> ids);

    /// <summary>
    /// 获取实体查询
    /// </summary>
    /// <returns>实体查询</returns>
    IQueryable<TEntity> GetQueryable();
} 