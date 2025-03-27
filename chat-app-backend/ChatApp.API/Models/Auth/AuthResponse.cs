namespace ChatApp.API.Models.Auth;

/// <summary>
/// 认证响应
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// JWT令牌
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserResponse User { get; set; }
}

/// <summary>
/// 用户信息响应
/// </summary>
public class UserResponse
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件地址
    /// </summary>
    public string Email { get; set; }
} 