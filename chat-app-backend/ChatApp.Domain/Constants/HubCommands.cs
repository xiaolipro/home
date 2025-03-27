namespace ChatApp.Domain.Constants;

/// <summary>
/// Hub命令常量
/// </summary>
public static class HubCommands
{
    /// <summary>
    /// 发送消息
    /// </summary>
    public const string SendMessage = "SendMessage";

    /// <summary>
    /// 接收消息
    /// </summary>
    public const string ReceiveMessage = "ReceiveMessage";

    /// <summary>
    /// 消息已读
    /// </summary>
    public const string MessageRead = "MessageRead";

    /// <summary>
    /// 消息撤回
    /// </summary>
    public const string MessageRecalled = "MessageRecalled";

    /// <summary>
    /// 用户上线
    /// </summary>
    public const string UserOnline = "UserOnline";

    /// <summary>
    /// 用户离线
    /// </summary>
    public const string UserOffline = "UserOffline";

    /// <summary>
    /// 用户正在输入
    /// </summary>
    public const string UserTyping = "UserTyping";

    /// <summary>
    /// 好友请求
    /// </summary>
    public const string FriendRequest = "FriendRequest";

    /// <summary>
    /// 好友请求已处理
    /// </summary>
    public const string FriendRequestHandled = "FriendRequestHandled";

    /// <summary>
    /// 好友状态变更
    /// </summary>
    public const string FriendStatusChanged = "FriendStatusChanged";
} 