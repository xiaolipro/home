using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.Message;

/// <summary>
/// 发送消息请求
/// </summary>
public class SendMessageDto
{
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
    /// 额外数据（如文件URL、图片URL等）
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
} 