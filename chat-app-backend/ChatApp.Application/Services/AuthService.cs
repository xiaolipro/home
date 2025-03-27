using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace ChatApp.Application.Services;

/// <summary>
/// 认证服务
/// </summary>
public class AuthService: IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="registerDto">注册信息</param>
    /// <returns>注册结果</returns>
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // 检查用户名是否已存在
        if (await _userRepository.GetByUsernameAsync(registerDto.Username) != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Username already exists"
            };
        }

        // 检查邮箱是否已存在
        if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        try
        {
            var passwordHash = BC.HashPassword(registerDto.Password);
            var user = new User(registerDto.Username, registerDto.Email, passwordHash);

            await _userRepository.AddAsync(user);

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                User = MapToUserDto(user)
            };
        }
        catch (ArgumentException ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginDto">登录信息</param>
    /// <returns>登录结果</returns>
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null || !BC.Verify(loginDto.Password, user.PasswordHash))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            User = MapToUserDto(user)
        };
    }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的访问令牌</returns>
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // TODO: 实现刷新令牌逻辑
        throw new NotImplementedException();
    }

    /// <summary>
    /// 生成JWT令牌
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    /// <summary>
    /// 将用户实体映射为DTO
    /// </summary>
    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
} 