using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Data;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public string? Avatar { get; set; }
    
    public DateTime? LastLoginAt { get; set; }

    public DateTime? LastActiveAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
} 