using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.API.Data;
using ChatApp.API.Models;
using ChatApp.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Services
{
    public class FriendshipService
    {
        private readonly ApplicationDbContext _context;

        public FriendshipService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FriendshipDto>> GetFriendsAsync(Guid userId)
        {
            var friendships = await _context.Friendships
                .Include(f => f.User)
                .Include(f => f.Friend)
                .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == FriendshipStatus.Accepted)
                .Select(f => new FriendshipDto
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    FriendId = f.FriendId,
                    Status = f.Status,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    User = new UserDto
                    {
                        Id = f.User.Id,
                        Username = f.User.Username,
                        Avatar = f.User.Avatar
                    },
                    Friend = new UserDto
                    {
                        Id = f.Friend.Id,
                        Username = f.Friend.Username,
                        Avatar = f.Friend.Avatar
                    }
                })
                .ToListAsync();

            return friendships;
        }

        public async Task<IEnumerable<FriendshipDto>> GetFriendRequestsAsync(Guid userId)
        {
            var requests = await _context.Friendships
                .Include(f => f.User)
                .Include(f => f.Friend)
                .Where(f => f.FriendId == userId && f.Status == FriendshipStatus.Pending)
                .Select(f => new FriendshipDto
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    FriendId = f.FriendId,
                    Status = f.Status,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    User = new UserDto
                    {
                        Id = f.User.Id,
                        Username = f.User.Username,
                        Avatar = f.User.Avatar
                    },
                    Friend = new UserDto
                    {
                        Id = f.Friend.Id,
                        Username = f.Friend.Username,
                        Avatar = f.Friend.Avatar
                    }
                })
                .ToListAsync();

            return requests;
        }

        public async Task<FriendshipDto> SendFriendRequestAsync(Guid userId, Guid friendId)
        {
            if (userId == friendId)
            {
                throw new InvalidOperationException("不能添加自己为好友");
            }

            var existingFriendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.UserId == userId && f.FriendId == friendId) ||
                    (f.UserId == friendId && f.FriendId == userId));

            if (existingFriendship != null)
            {
                if (existingFriendship.Status == FriendshipStatus.Accepted)
                {
                    throw new InvalidOperationException("已经是好友关系");
                }
                if (existingFriendship.Status == FriendshipStatus.Pending)
                {
                    throw new InvalidOperationException("已经发送过好友请求");
                }
            }

            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return await GetFriendshipDtoAsync(friendship.Id);
        }

        public async Task AcceptFriendRequestAsync(Guid userId, Guid requestId)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == requestId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);

            if (friendship == null)
            {
                throw new InvalidOperationException("好友请求不存在或已被处理");
            }

            friendship.Status = FriendshipStatus.Accepted;
            friendship.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task RejectFriendRequestAsync(Guid userId, Guid requestId)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == requestId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);

            if (friendship == null)
            {
                throw new InvalidOperationException("好友请求不存在或已被处理");
            }

            friendship.Status = FriendshipStatus.Rejected;
            friendship.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriendshipAsync(Guid userId, Guid friendshipId)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId && 
                    (f.UserId == userId || f.FriendId == userId));

            if (friendship == null)
            {
                throw new InvalidOperationException("好友关系不存在");
            }

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }

        private async Task<FriendshipDto> GetFriendshipDtoAsync(Guid friendshipId)
        {
            var friendship = await _context.Friendships
                .Include(f => f.User)
                .Include(f => f.Friend)
                .FirstOrDefaultAsync(f => f.Id == friendshipId);

            if (friendship == null)
            {
                throw new InvalidOperationException("好友关系不存在");
            }

            return new FriendshipDto
            {
                Id = friendship.Id,
                UserId = friendship.UserId,
                FriendId = friendship.FriendId,
                Status = friendship.Status,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = friendship.UpdatedAt,
                User = new UserDto
                {
                    Id = friendship.User.Id,
                    Username = friendship.User.Username,
                    Avatar = friendship.User.Avatar
                },
                Friend = new UserDto
                {
                    Id = friendship.Friend.Id,
                    Username = friendship.Friend.Username,
                    Avatar = friendship.Friend.Avatar
                }
            };
        }
    }
} 