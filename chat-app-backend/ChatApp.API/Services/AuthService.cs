using ChatApp.API.Data;
using ChatApp.API.Models.Auth;
using ChatApp.API.Models.DTOs;
using ChatApp.API.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ChatApp.API.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly CaptchaService _captchaService;
        private readonly IWebHostEnvironment _environment;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration,
            CaptchaService captchaService,
            IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _captchaService = captchaService;
            _environment = environment;
        }

        public async Task<CaptchaResponse> GetCaptchaAsync()
        {
            var (code, imageBase64) = await _captchaService.GenerateCaptchaAsync();
            var captchaId = Guid.NewGuid().ToString();
            
            await _captchaService.SetCaptchaAsync(captchaId, code);

            return new CaptchaResponse
            {
                ImageBase64 = imageBase64,
                CaptchaId = captchaId
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // 验证验证码
            if (!await _captchaService.ValidateCaptchaAsync(request.CaptchaId, request.CaptchaCode))
            {
                throw new BusinessException("INVALID_CAPTCHA", "验证码错误或已过期");
            }

            // 检查用户是否存在
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new BusinessException("EMAIL_EXISTS", "该邮箱已被注册");
            }

            // 创建新用户
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 生成令牌
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                }
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // 验证验证码
            if (!await _captchaService.ValidateCaptchaAsync(request.CaptchaId, request.CaptchaCode))
            {
                throw new BusinessException("INVALID_CAPTCHA", "验证码错误或已过期");
            }

            // 查找用户
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                throw new BusinessException("USER_NOT_FOUND", "用户不存在");
            }

            // 验证密码
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new BusinessException("INVALID_PASSWORD", "密码错误");
            }

            // 更新最后登录时间
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 生成令牌
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    LastLoginAt = user.LastLoginAt,
                    LastActiveAt = user.LastActiveAt,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };
        }

        public async Task<AuthResponse> GetCurrentUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new BusinessException("USER_NOT_FOUND", "用户不存在");
            }

            return new AuthResponse
            {
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    LastLoginAt = user.LastLoginAt,
                    LastActiveAt = user.LastActiveAt,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new BusinessException("USER_NOT_FOUND", "用户不存在");
            }

            // 更新最后活跃时间
            user.LastActiveAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        private string GenerateJwtToken(User user)
        {
            if (string.IsNullOrEmpty(_configuration["JWT:Secret"]))
            {
                throw new BusinessException("JWT_CONFIG_ERROR", "JWT配置错误");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

        /// <summary>
        /// 上传用户头像
        /// </summary>
        public async Task<AuthResponse> UploadAvatarAsync(Guid userId, IFormFile file)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new BusinessException("USER_NOT_FOUND", "用户不存在");
            }

            // 生成文件名
            var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
            
            // 确保目录存在
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 更新用户头像
            user.Avatar = $"/uploads/avatars/{fileName}";
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    LastLoginAt = user.LastLoginAt,
                    LastActiveAt = user.LastActiveAt,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };
        }
    }
} 