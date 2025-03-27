namespace ChatApp.Domain.Enums;

/// <summary>
/// 消息类型
/// </summary>
public enum MessageType
{
    /// <summary>
    /// 文本消息
    /// </summary>
    Text,

    /// <summary>
    /// 图片消息
    /// </summary>
    Image,

    /// <summary>
    /// 文件消息
    /// </summary>
    File,

    /// <summary>
    /// 语音消息
    /// </summary>
    Voice,

    /// <summary>
    /// 视频消息
    /// </summary>
    Video,

    /// <summary>
    /// 系统消息
    /// </summary>
    System
} 