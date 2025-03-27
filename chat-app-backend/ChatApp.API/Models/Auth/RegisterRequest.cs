using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Models.Auth;

/// <summary>
/// 用户注册请求
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// 用户名，最大长度50个字符
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    /// <summary>
    /// 电子邮件地址
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// 密码，最小长度6个字符，最大长度100个字符
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }

    /// <summary>
    /// 确认密码，必须与密码相同
    /// </summary>
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

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