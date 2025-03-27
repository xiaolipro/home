namespace ChatApp.Domain.Enums;

/// <summary>
/// 好友关系状态
/// </summary>
public enum FriendshipStatus
{
    /// <summary>
    /// 待确认
    /// </summary>
    Pending,

    /// <summary>
    /// 已接受
    /// </summary>
    Accepted,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected,

    /// <summary>
    /// 已删除
    /// </summary>
    Deleted,

    /// <summary>
    /// 已拉黑
    /// </summary>
    Blocked
} 