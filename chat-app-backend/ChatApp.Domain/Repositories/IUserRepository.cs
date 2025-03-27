using ChatApp.Domain.Entities;

namespace ChatApp.Domain.Repositories;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface IUserRepository: IBaseRepository<User, Guid>
{
    /// <summary>
    /// 根据用户名查找用户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns>用户实体</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// 根据邮箱查找用户
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>用户实体</returns>
    Task<User?> GetByEmailAsync(string email);
} 