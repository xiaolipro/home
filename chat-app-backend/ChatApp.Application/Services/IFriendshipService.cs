using ChatApp.Application.DTOs.Friendship;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Services;

/// <summary>
/// 好友服务接口
/// </summary>
public interface IFriendshipService
{
    /// <summary>
    /// 获取好友列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>好友列表</returns>
    Task<IEnumerable<FriendshipDto>> GetFriendsAsync(Guid userId);

    /// <summary>
    /// 获取待确认的好友请求
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>待确认的好友请求列表</returns>
    Task<IEnumerable<FriendshipDto>> GetPendingRequestsAsync(Guid userId);

    /// <summary>
    /// 发送好友请求
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="request">好友请求</param>
    /// <returns>好友关系</returns>
    Task<FriendshipDto> SendFriendRequestAsync(Guid userId, AddFriendRequestDto request);

    /// <summary>
    /// 处理好友请求
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="friendshipId">好友关系ID</param>
    /// <param name="status">处理状态</param>
    /// <returns>好友关系</returns>
    Task<FriendshipDto> HandleFriendRequestAsync(Guid userId, Guid friendshipId, FriendshipStatus status);

    /// <summary>
    /// 更新好友关系
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="friendshipId">好友关系ID</param>
    /// <param name="remark">备注名</param>
    /// <param name="group">分组</param>
    /// <param name="isPinned">是否置顶</param>
    /// <param name="isMuted">是否免打扰</param>
    /// <returns>好友关系</returns>
    Task<FriendshipDto> UpdateFriendshipAsync(
        Guid userId,
        Guid friendshipId,
        string? remark = null,
        string? group = null,
        bool? isPinned = null,
        bool? isMuted = null);

    /// <summary>
    /// 删除好友
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="friendshipId">好友关系ID</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteFriendAsync(Guid userId, Guid friendshipId);
} 