namespace ChatApp.API.Models.Auth;

/// <summary>
/// 认证响应
/// </summary>
public class AuthResponse
{
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
    public Guid Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件地址
    /// </summary>
    public string Email { get; set; }
    
    public string? Avatar { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 