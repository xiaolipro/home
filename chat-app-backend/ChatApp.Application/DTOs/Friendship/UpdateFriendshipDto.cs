namespace ChatApp.Application.DTOs.Friendship;

/// <summary>
/// 更新好友关系DTO
/// </summary>
public class UpdateFriendshipDto
{
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
    public bool? IsPinned { get; set; }

    /// <summary>
    /// 是否免打扰
    /// </summary>
    public bool? IsMuted { get; set; }
} 