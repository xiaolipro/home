using Microsoft.AspNetCore.Mvc;
using ChatApp.API.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ChatApp.API.Services;

namespace ChatApp.API.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns>验证码图片和ID</returns>
    [HttpGet("captcha")]
    [ProducesResponseType(typeof(CaptchaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CaptchaResponse>> GetCaptcha()
    {
        var response = await _authService.GetCaptchaAsync();
        return Ok(response);
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="request">注册请求</param>
    /// <returns>注册结果</returns>
    /// <response code="201">注册成功</response>
    /// <response code="400">注册失败，邮箱已存在</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(GetCurrentUser), new { }, response);
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    /// <response code="200">登录成功</response>
    /// <response code="400">登录失败，用户名或密码错误</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>当前用户信息</returns>
    /// <response code="200">获取成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">用户不存在</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthResponse>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized();
        }

        var response = await _authService.GetCurrentUserAsync(userId);
        return Ok(response);
    }
} 