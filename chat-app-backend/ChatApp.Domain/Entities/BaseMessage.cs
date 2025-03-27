using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

/// <summary>
/// 消息基类
/// </summary>
public abstract class BaseMessage
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 发送者ID
    /// </summary>
    public Guid SenderId { get; set; }

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
    /// 已读时间
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
} 