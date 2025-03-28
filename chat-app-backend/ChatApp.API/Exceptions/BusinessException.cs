namespace ChatApp.API.Exceptions;

/// <summary>
/// 业务逻辑异常
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; }

    public BusinessException(string code, string message) : base(message)
    {
        Code = code;
        Message = message;
    }
} 