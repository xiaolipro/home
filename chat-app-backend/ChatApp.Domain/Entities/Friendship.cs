using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

/// <summary>
/// 好友关系
/// </summary>
public class Friendship
{
    /// <summary>
    /// 好友关系ID
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 好友ID
    /// </summary>
    public Guid FriendId { get; set; }

    /// <summary>
    /// 好友关系状态
    /// </summary>
    public FriendshipStatus Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 备注名
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 分组
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// 是否免打扰
    /// </summary>
    public bool IsMuted { get; set; }
} 