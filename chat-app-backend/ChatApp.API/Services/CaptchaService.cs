using System.Security.Cryptography;
using SkiaSharp;
using System.Text;
using StackExchange.Redis;

namespace ChatApp.API.Services;

/// <summary>
/// 验证码服务，负责生成、存储和验证验证码
/// </summary>
public class CaptchaService
{
    private readonly Random _random = new();
    private IDatabase Db => _redis.GetDatabase();
    private readonly IConnectionMultiplexer _redis;
    private const string CaptchaKeyPrefix = "captcha:";
    
    // 验证码配置常量
    private const int CaptchaLength = 4;
    private const int ImageWidth = 120;
    private const int ImageHeight = 40;
    private const int FontSize = 24;
    private const int ExpiryMinutes = 5;
    private const int InterferenceLines = 5;
    private const int InterferenceDots = 100;
    private const int MaxRotationDegrees = 10;
    private const int CharSpacing = 25;

    /// <summary>
    /// 初始化验证码服务
    /// </summary>
    /// <param name="redis">Redis连接实例</param>
    public CaptchaService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    /// <summary>
    /// 生成新的验证码
    /// </summary>
    /// <returns>包含验证码文本和图片Base64字符串的元组</returns>
    public async Task<(string Code, string ImageBase64)> GenerateCaptchaAsync()
    {
        // 生成随机验证码
        var code = GenerateRandomCode();
        
        // 生成验证码图片
        var imageBase64 = GenerateCaptchaImage(code);

        return (code, imageBase64);
    }

    /// <summary>
    /// 验证用户输入的验证码
    /// </summary>
    /// <param name="captchaId">验证码ID</param>
    /// <param name="code">用户输入的验证码</param>
    /// <returns>验证是否通过</returns>
    public async Task<bool> ValidateCaptchaAsync(string captchaId, string code)
    {
        if (string.IsNullOrEmpty(captchaId) || string.IsNullOrEmpty(code))
            return false;

        var key = CaptchaKeyPrefix + captchaId;
        var storedCode = await Db.StringGetAsync(key);
        if (storedCode.IsNullOrEmpty)
            return false;

        var isValid = storedCode.ToString().Equals(code, StringComparison.OrdinalIgnoreCase);
        if (isValid)
        {
            // 验证成功后删除验证码，防止重复使用
            await Db.KeyDeleteAsync(key);
        }

        return isValid;
    }

    /// <summary>
    /// 将验证码存储到Redis中
    /// </summary>
    /// <param name="captchaId">验证码ID</param>
    /// <param name="code">验证码文本</param>
    public async Task SetCaptchaAsync(string captchaId, string code)
    {
        var key = CaptchaKeyPrefix + captchaId;
        // 设置过期时间
        await Db.StringSetAsync(key, code, TimeSpan.FromMinutes(ExpiryMinutes));
    }

    /// <summary>
    /// 生成随机验证码文本
    /// </summary>
    /// <returns>4位随机验证码</returns>
    private string GenerateRandomCode()
    {
        // 使用易识别的字符，排除容易混淆的字符（如0、O、1、I等）
        const string chars = "2345678ABCDEFGHJKLMNPQRSTUVWXYZ";
        var code = new StringBuilder();
        for (int i = 0; i < CaptchaLength; i++)
        {
            code.Append(chars[_random.Next(chars.Length)]);
        }
        return code.ToString();
    }

    /// <summary>
    /// 生成验证码图片
    /// </summary>
    /// <param name="code">验证码文本</param>
    /// <returns>图片的Base64字符串</returns>
    private string GenerateCaptchaImage(string code)
    {
        // 创建位图
        using var bitmap = new SKBitmap(ImageWidth, ImageHeight);
        using var canvas = new SKCanvas(bitmap);
        
        // 设置白色背景
        canvas.Clear(SKColors.White);
        
        // 添加随机干扰线
        using var linePaint = new SKPaint
        {
            Color = SKColors.LightGray,
            StrokeWidth = 1
        };
        
        for (int i = 0; i < InterferenceLines; i++)
        {
            var startX = _random.Next(0, ImageWidth);
            var startY = _random.Next(0, ImageHeight);
            var endX = _random.Next(0, ImageWidth);
            var endY = _random.Next(0, ImageHeight);
            canvas.DrawLine(startX, startY, endX, endY, linePaint);
        }

        // 添加随机干扰点
        using var dotPaint = new SKPaint
        {
            Color = SKColors.LightGray,
            Style = SKPaintStyle.Fill
        };

        for (int i = 0; i < InterferenceDots; i++)
        {
            var x = _random.Next(0, ImageWidth);
            var y = _random.Next(0, ImageHeight);
            canvas.DrawCircle(x, y, 1, dotPaint);
        }
        
        // 计算文字尺寸和位置
        var textWidth = 0f;
        var textHeight = 0f;
        using (var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = FontSize,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
        })
        {
            // 计算所有字符的总宽度和最大高度
            foreach (var c in code)
            {
                var bounds = new SKRect();
                textPaint.MeasureText(c.ToString(), ref bounds);
                textWidth += bounds.Width;
                textHeight = Math.Max(textHeight, bounds.Height);
            }
        }

        // 计算起始位置使文字在图片中居中显示
        var startXf = (ImageWidth - textWidth) / code.Length;
        var startYf = (ImageHeight + textHeight) / 2;

        // 绘制每个字符，添加随机旋转效果
        foreach (var c in code)
        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = FontSize,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            // 保存当前画布状态
            canvas.Save();
            
            // 移动到当前字符的绘制位置
            canvas.Translate(startXf, startYf);
            
            // 添加随机旋转
            canvas.RotateDegrees(_random.Next(-MaxRotationDegrees, MaxRotationDegrees));
            
            // 绘制字符
            canvas.DrawText(c.ToString(), 0, 0, textPaint);
            
            // 恢复画布状态
            canvas.Restore();
            
            // 移动到下一个字符位置
            startXf += CharSpacing;
        }
        
        // 将位图转换为Base64字符串
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream();
        data.SaveTo(ms);
        return Convert.ToBase64String(ms.ToArray());
    }
} 