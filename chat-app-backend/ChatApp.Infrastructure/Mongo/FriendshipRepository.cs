using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Repositories;
using ChatApp.Infrastructure.Repositories;
using MongoDB.Driver;

namespace ChatApp.Infrastructure.Mongo;

/// <summary>
/// MongoDB好友关系仓储实现
/// </summary>
public class FriendshipRepository : MongoBaseRepository<Friendship, Guid>, IFriendshipRepository
{
    public FriendshipRepository(IMongoDatabase database) : base(database, "friendships")
    {
    }

    protected override Guid GetEntityId(Friendship entity)
    {
        return entity.Id;
    }

    public async Task<IEnumerable<Friendship>> GetFriendsAsync(Guid userId)
    {
        return await Collection
            .Find(f => f.UserId == userId && f.Status == FriendshipStatus.Accepted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Friendship>> GetPendingRequestsAsync(Guid userId)
    {
        return await Collection
            .Find(f => f.FriendId == userId && f.Status == FriendshipStatus.Pending)
            .ToListAsync();
    }

    public async Task<Friendship?> GetFriendshipAsync(Guid userId, Guid friendId)
    {
        return await Collection
            .Find(f => (f.UserId == userId && f.FriendId == friendId) ||
                      (f.UserId == friendId && f.FriendId == userId))
            .FirstOrDefaultAsync();
    }

    public async Task<Friendship> UpdateStatusAsync(Guid friendshipId, FriendshipStatus status)
    {
        var update = Builders<Friendship>.Update
            .Set(f => f.Status, status)
            .Set(f => f.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<Friendship>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await Collection
            .FindOneAndUpdateAsync(
                f => f.Id == friendshipId,
                update,
                options) ?? throw new InvalidOperationException("Friendship not found");
    }

    public async Task<bool> AreFriendsAsync(Guid userId, Guid friendId)
    {
        return await Collection
            .Find(f => (f.UserId == userId && f.FriendId == friendId) ||
                      (f.UserId == friendId && f.FriendId == userId))
            .AnyAsync(f => f.Status == FriendshipStatus.Accepted);
    }
} 