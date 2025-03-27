using Microsoft.AspNetCore.Mvc;
using ChatApp.API.Data;
using ChatApp.API.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ChatApp.API.Services;

namespace ChatApp.API.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly CaptchaService _captchaService;

    public AuthController(
        ApplicationDbContext context, 
        IConfiguration configuration, 
        CaptchaService captchaService)
    {
        _context = context;
        _configuration = configuration;
        _captchaService = captchaService;
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns>验证码图片和ID</returns>
    [HttpGet("captcha")]
    [ProducesResponseType(typeof(CaptchaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CaptchaResponse>> GetCaptcha()
    {
        var (code, imageBase64) = await _captchaService.GenerateCaptchaAsync();
        var captchaId = Guid.NewGuid().ToString();
        
        // 存储验证码，5分钟有效期
        await _captchaService.SetCaptchaAsync(captchaId, code);

        return Ok(new CaptchaResponse
        {
            ImageBase64 = imageBase64,
            CaptchaId = captchaId
        });
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="registerRequest">注册请求</param>
    /// <returns>注册结果</returns>
    /// <response code="200">注册成功</response>
    /// <response code="400">注册失败，邮箱已存在</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest registerRequest)
    {
        // 验证验证码
        if (!await _captchaService.ValidateCaptchaAsync(registerRequest.CaptchaId, registerRequest.CaptchaCode))
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "验证码错误或已过期"
            });
        }

        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.Email == registerRequest.Email))
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "User with this email already exists"
            });
        }

        // Create new user
        var user = new User
        {
            Username = registerRequest.Username,
            Email = registerRequest.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate token
        var token = GenerateJwtToken(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Registration successful",
            Token = token,
            User = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        });
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginRequest">登录请求</param>
    /// <returns>登录结果</returns>
    /// <response code="200">登录成功</response>
    /// <response code="400">登录失败，用户名或密码错误</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest loginRequest)
    {
        // 验证验证码
        if (!await _captchaService.ValidateCaptchaAsync(loginRequest.CaptchaId, loginRequest.CaptchaCode))
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "验证码错误或已过期"
            });
        }

        // Find user
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
        if (user == null)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "User not found"
            });
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Invalid password"
            });
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generate token
        var token = GenerateJwtToken(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            User = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        });
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>当前用户信息</returns>
    /// <response code="200">获取成功</response>
    /// <response code="401">未授权</response>
    /// <response code="400">用户不存在</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "User not found"
            });
        }

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "User retrieved successfully",
            User = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 