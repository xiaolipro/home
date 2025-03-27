namespace ChatApp.Domain.Enums;

/// <summary>
/// 消息状态
/// </summary>
public enum MessageStatus
{
    /// <summary>
    /// 已发送
    /// </summary>
    Sent,

    /// <summary>
    /// 已送达
    /// </summary>
    Delivered,

    /// <summary>
    /// 已读
    /// </summary>
    Read,

    /// <summary>
    /// 已撤回
    /// </summary>
    Recalled
} 