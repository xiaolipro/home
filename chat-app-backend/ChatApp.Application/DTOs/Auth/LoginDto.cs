using System.ComponentModel.DataAnnotations;

namespace ChatApp.Application.DTOs.Auth;

/// <summary>
/// 登录信息
/// </summary>
public class LoginDto
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
} 