namespace ChatApp.Application.DTOs.Auth;

/// <summary>
/// 认证响应数据传输对象
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// JWT令牌
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserDto User { get; set; }
} 