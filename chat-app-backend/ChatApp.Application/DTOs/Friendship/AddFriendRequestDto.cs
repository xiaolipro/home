namespace ChatApp.Application.DTOs.Friendship;

/// <summary>
/// 添加好友请求DTO
/// </summary>
public class AddFriendRequestDto
{
    /// <summary>
    /// 好友ID
    /// </summary>
    public Guid FriendId { get; set; }

    /// <summary>
    /// 备注名
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 分组
    /// </summary>
    public string? Group { get; set; }
} 