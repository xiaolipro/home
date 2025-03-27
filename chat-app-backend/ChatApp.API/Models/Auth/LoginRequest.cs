using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Models.Auth;

/// <summary>
/// 用户登录请求
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 电子邮件地址
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// 验证码ID
    /// </summary>
    [Required]
    public string CaptchaId { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string CaptchaCode { get; set; }
} 