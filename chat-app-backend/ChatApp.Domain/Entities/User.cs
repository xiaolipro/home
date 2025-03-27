using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Username { get; private set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; private set; }

    /// <summary>
    /// 密码哈希
    /// </summary>
    [Required]
    public string PasswordHash { get; private set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string? Avatar { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime LastLoginAt { get; private set; }

    private User() { }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="email">邮箱</param>
    /// <param name="passwordHash">密码哈希</param>
    public User(string username, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        LastLoginAt = CreatedAt;
    }

    /// <summary>
    /// 更新最后登录时间
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
} 