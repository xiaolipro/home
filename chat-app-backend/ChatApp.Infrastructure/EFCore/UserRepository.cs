using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;
using ChatApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.EFCore;

/// <summary>
/// EF Core用户仓储实现
/// </summary>
public class UserRepository : EfCoreBaseRepository<User, Guid>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }

    protected override Guid GetEntityId(User entity)
    {
        return entity.Id;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
} 