namespace ChatApp.API.Models.Auth;

/// <summary>
/// 验证码响应
/// </summary>
public class CaptchaResponse
{
    /// <summary>
    /// 验证码图片（Base64格式）
    /// </summary>
    public string ImageBase64 { get; set; }

    /// <summary>
    /// 验证码ID（用于验证）
    /// </summary>
    public string CaptchaId { get; set; }
} 