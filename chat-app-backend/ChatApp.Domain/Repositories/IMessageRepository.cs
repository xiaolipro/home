using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Repositories;

/// <summary>
/// 私聊消息仓库接口
/// </summary>
public interface IMessageRepository : IBaseRepository<Message, Guid>
{
    /// <summary>
    /// 获取两个用户之间的消息历史
    /// </summary>
    /// <param name="userId1">用户1 ID</param>
    /// <param name="userId2">用户2 ID</param>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>消息列表</returns>
    Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2, int page = 1, int pageSize = 20);

    /// <summary>
    /// 获取用户未读消息数量
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>未读消息数量</returns>
    Task<int> GetUnreadCountAsync(Guid userId);

    /// <summary>
    /// 更新消息状态
    /// </summary>
    /// <param name="messageId">消息ID</param>
    /// <param name="status">新状态</param>
    /// <returns>更新后的消息</returns>
    Task<Message> UpdateStatusAsync(Guid messageId, MessageStatus status);

    /// <summary>
    /// 撤回消息
    /// </summary>
    /// <param name="messageId">消息ID</param>
    /// <param name="userId">用户ID</param>
    /// <returns>是否成功</returns>
    Task<bool> RecallMessageAsync(Guid messageId, Guid userId);
} 