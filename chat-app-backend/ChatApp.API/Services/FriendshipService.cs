using ChatApp.API.Data;
using ChatApp.API.Models;
using ChatApp.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChatApp.API.Exceptions;

namespace ChatApp.API.Services
{
    public class FriendshipService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FriendshipService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 获取当前用户的好友列表
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetFriendsAsync()
        {
            var userId = GetCurrentUserId();
            var friendships = await _context.Friendships
                .Include(f => f.User)
                .Include(f => f.Friend)
                .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            return friendships.Select(f => userId == f.UserId
                ? new UserDto
                {
                    Id = f.FriendId,
                    Username = f.Friend.Username,
                    Email = f.Friend.Email,
                    Avatar = f.Friend.Avatar,
                    LastLoginAt = f.Friend.LastLoginAt,
                    LastActiveAt = f.Friend.LastActiveAt,
                    CreatedAt = f.Friend.CreatedAt,
                    UpdatedAt = f.Friend.UpdatedAt
                }
                : new UserDto
                {
                    Id = f.UserId,
                    Username = f.User.Username,
                    Email = f.User.Email,
                    Avatar = f.User.Avatar,
                    LastLoginAt = f.User.LastLoginAt,
                    LastActiveAt = f.User.LastActiveAt,
                    CreatedAt = f.User.CreatedAt,
                    UpdatedAt = f.User.UpdatedAt
                });
        }

        public async Task<IEnumerable<FriendshipDto>> GetPendingRequestsAsync()
        {
            var userId = GetCurrentUserId();
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
                    User = new UserDto
                    {
                        Id = f.User.Id,
                        Username = f.User.Username,
                        Email = f.User.Email,
                        Avatar = f.User.Avatar,
                        CreatedAt = f.User.CreatedAt,
                        LastActiveAt = f.User.LastActiveAt,
                        LastLoginAt = f.User.LastLoginAt,
                        UpdatedAt = f.User.UpdatedAt
                    },
                    Friend = new UserDto
                    {
                        Id = f.Friend.Id,
                        Username = f.Friend.Username,
                        Email = f.Friend.Email,
                        Avatar = f.Friend.Avatar,
                        CreatedAt = f.Friend.CreatedAt,
                        LastActiveAt = f.Friend.LastActiveAt,
                        LastLoginAt = f.Friend.LastLoginAt,
                        UpdatedAt = f.User.UpdatedAt
                    }
                })
                .ToListAsync();

            return requests;
        }

        public async Task<FriendshipDto> SendFriendRequestAsync(Guid senderId)
        {
            // 查找目标用户
            var targetUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == senderId);

            if (targetUser == null)
            {
                throw new BusinessException("USER_NOT_FOUND", "未找到指定用户");
            }

            if (targetUser.Id == senderId)
            {
                throw new BusinessException("SELF_FRIEND_REQUEST", "不能添加自己为好友");
            }

            // 检查是否已经是好友
            var existingFriendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.UserId == senderId && f.FriendId == targetUser.Id) ||
                    (f.UserId == targetUser.Id && f.FriendId == senderId));

            if (existingFriendship != null)
            {
                if (existingFriendship.Status == FriendshipStatus.Accepted)
                {
                    throw new BusinessException("ALREADY_FRIENDS", "已经是好友关系");
                }

                if (existingFriendship.Status == FriendshipStatus.Pending)
                {
                    throw new BusinessException("REQUEST_EXISTS", "已经发送过好友请求");
                }
            }

            // 创建新的好友请求
            var friendship = new Friendship
            {
                UserId = senderId,
                FriendId = targetUser.Id,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return await GetFriendshipDtoAsync(friendship.Id);
        }

        public async Task AcceptFriendRequestAsync(Guid requestId)
        {
            var userId = GetCurrentUserId();
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    f.Id == requestId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);

            if (friendship == null)
            {
                throw new BusinessException("REQUEST_NOT_FOUND", "未找到指定的好友请求");
            }

            friendship.Status = FriendshipStatus.Accepted;
            await _context.SaveChangesAsync();
        }

        public async Task RejectFriendRequestAsync(Guid requestId)
        {
            var userId = GetCurrentUserId();
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    f.Id == requestId && f.FriendId == userId && f.Status == FriendshipStatus.Pending);

            if (friendship == null)
            {
                throw new BusinessException("REQUEST_NOT_FOUND", "未找到指定的好友请求");
            }

            friendship.Status = FriendshipStatus.Rejected;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFriendshipAsync(Guid friendshipId)
        {
            var userId = GetCurrentUserId();
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                                          (f.UserId == userId || f.FriendId == userId) &&
                                          f.Status == FriendshipStatus.Accepted);

            if (friendship == null)
            {
                throw new BusinessException("FRIENDSHIP_NOT_FOUND", "未找到指定的好友关系");
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
                throw new BusinessException("REQUEST_NOT_FOUND", "好友关系不存在");
            }

            return new FriendshipDto
            {
                Id = friendship.Id,
                UserId = friendship.UserId,
                FriendId = friendship.FriendId,
                Status = friendship.Status,
                CreatedAt = friendship.CreatedAt,
                User = new UserDto
                {
                    Id = friendship.User.Id,
                    Username = friendship.User.Username,
                    Email = friendship.User.Email,
                    Avatar = friendship.User.Avatar,
                    CreatedAt = friendship.User.CreatedAt,
                    LastActiveAt = friendship.User.LastActiveAt,
                    LastLoginAt = friendship.User.LastLoginAt,
                    UpdatedAt = friendship.User.UpdatedAt
                },
                Friend = new UserDto
                {
                    Id = friendship.Friend.Id,
                    Username = friendship.Friend.Username,
                    Email = friendship.Friend.Email,
                    Avatar = friendship.Friend.Avatar,
                    CreatedAt = friendship.Friend.CreatedAt,
                    LastActiveAt = friendship.Friend.LastActiveAt,
                    LastLoginAt = friendship.Friend.LastLoginAt,
                    UpdatedAt = friendship.User.UpdatedAt
                }
            };
        }

        public async Task<PagedResult<UserDto>> SearchUsersAsync(string query, int page, int pageSize)
        {
            var userId = GetCurrentUserId();

            // 获取总记录数
            var totalCount = await _context.Users
                .Where(u => u.Id != userId && // 排除自己
                            (u.Username.StartsWith(query) || u.Email.StartsWith(query)))
                .CountAsync();

            // 获取分页数据
            var users = await _context.Users
                .Where(u => u.Id != userId && // 排除自己
                            (u.Username.StartsWith(query) || u.Email.StartsWith(query)))
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    CreatedAt = u.CreatedAt,
                    LastActiveAt = u.LastActiveAt,
                    LastLoginAt = u.LastLoginAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<UserDto>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        protected Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                throw new UnauthorizedAccessException("未授权的访问");
            }

            return userGuid;
        }
    }
}