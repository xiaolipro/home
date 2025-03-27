using System.ComponentModel.DataAnnotations;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

/// <summary>
/// 消息实体
/// </summary>
public class Message
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
    /// 发送者
    /// </summary>
    public User Sender { get; set; }

    /// <summary>
    /// 接收者ID（私聊时为接收用户ID，群聊时为群组ID）
    /// </summary>
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// 接收者（私聊时为接收用户，群聊时为群组）
    /// </summary>
    public object Receiver { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageType Type { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    public MessageStatus Status { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 送达时间
    /// </summary>
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 撤回时间
    /// </summary>
    public DateTime? RecalledAt { get; set; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}