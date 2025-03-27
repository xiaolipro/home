using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.Message;

/// <summary>
/// 消息数据传输对象
/// </summary>
public class MessageDto
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 发送者ID
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// 接收者ID
    /// </summary>
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageType Type { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息状态
    /// </summary>
    public MessageStatus Status { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 额外数据
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
} 