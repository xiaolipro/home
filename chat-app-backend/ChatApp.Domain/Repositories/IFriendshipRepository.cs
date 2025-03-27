using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Repositories;

/// <summary>
/// 好友关系仓储接口
/// </summary>
public interface IFriendshipRepository: IBaseRepository<Friendship, Guid>
{
    /// <summary>
    /// 获取用户的好友列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>好友关系列表</returns>
    Task<IEnumerable<Friendship>> GetFriendsAsync(Guid userId);

    /// <summary>
    /// 获取待确认的好友请求
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>待确认的好友请求列表</returns>
    Task<IEnumerable<Friendship>> GetPendingRequestsAsync(Guid userId);

    /// <summary>
    /// 获取两个用户之间的好友关系
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="friendId">好友ID</param>
    /// <returns>好友关系</returns>
    Task<Friendship?> GetFriendshipAsync(Guid userId, Guid friendId);

    /// <summary>
    /// 更新好友关系状态
    /// </summary>
    /// <param name="friendshipId">好友关系ID</param>
    /// <param name="status">新状态</param>
    /// <returns>更新后的好友关系</returns>
    Task<Friendship> UpdateStatusAsync(Guid friendshipId, FriendshipStatus status);

    /// <summary>
    /// 检查两个用户是否是好友
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="friendId">好友ID</param>
    /// <returns>是否是好友</returns>
    Task<bool> AreFriendsAsync(Guid userId, Guid friendId);
} 