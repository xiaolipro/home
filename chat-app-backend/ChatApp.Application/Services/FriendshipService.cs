using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.DTOs.Friendship;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Repositories;

namespace ChatApp.Application.Services;

/// <summary>
/// 好友服务实现
/// </summary>
public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;

    public FriendshipService(IFriendshipRepository friendshipRepository, IUserRepository userRepository)
    {
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<FriendshipDto>> GetFriendsAsync(Guid userId)
    {
        var friendships = await _friendshipRepository.GetFriendsAsync(userId);
        return await MapToDtosAsync(friendships);
    }

    public async Task<IEnumerable<FriendshipDto>> GetPendingRequestsAsync(Guid userId)
    {
        var friendships = await _friendshipRepository.GetPendingRequestsAsync(userId);
        return await MapToDtosAsync(friendships);
    }

    public async Task<FriendshipDto> SendFriendRequestAsync(Guid userId, AddFriendRequestDto request)
    {
        // 检查是否已经是好友
        if (await _friendshipRepository.AreFriendsAsync(userId, request.FriendId))
        {
            throw new InvalidOperationException("Already friends");
        }

        // 检查是否已经存在好友请求
        var existingFriendship = await _friendshipRepository.GetFriendshipAsync(userId, request.FriendId);
        if (existingFriendship != null && existingFriendship.Status == FriendshipStatus.Pending)
        {
            throw new InvalidOperationException("Friend request already sent");
        }

        var friendship = new Friendship
        {
            UserId = userId,
            FriendId = request.FriendId,
            Status = FriendshipStatus.Pending,
            Remark = request.Remark,
            Group = request.Group
        };

        var savedFriendship = await _friendshipRepository.AddAsync(friendship);
        return await MapToDtoAsync(savedFriendship);
    }

    public async Task<FriendshipDto> HandleFriendRequestAsync(Guid userId, Guid friendshipId, FriendshipStatus status)
    {
        var friendship = await _friendshipRepository.GetFriendshipAsync(userId, friendshipId);
        if (friendship == null)
        {
            throw new InvalidOperationException("Friendship not found");
        }

        if (friendship.Status != FriendshipStatus.Pending)
        {
            throw new InvalidOperationException("Friendship request already handled");
        }

        var updatedFriendship = await _friendshipRepository.UpdateStatusAsync(friendshipId, status);
        return await MapToDtoAsync(updatedFriendship);
    }

    public async Task<FriendshipDto> UpdateFriendshipAsync(
        Guid userId,
        Guid friendshipId,
        string? remark = null,
        string? group = null,
        bool? isPinned = null,
        bool? isMuted = null)
    {
        var friendship = await _friendshipRepository.GetFriendshipAsync(userId, friendshipId);
        if (friendship == null)
        {
            throw new InvalidOperationException("Friendship not found");
        }

        if (friendship.Status != FriendshipStatus.Accepted)
        {
            throw new InvalidOperationException("Can only update accepted friendships");
        }

        if (remark != null) friendship.Remark = remark;
        if (group != null) friendship.Group = group;
        if (isPinned.HasValue) friendship.IsPinned = isPinned.Value;
        if (isMuted.HasValue) friendship.IsMuted = isMuted.Value;

        var updatedFriendship = await _friendshipRepository.UpdateAsync(friendship);
        return await MapToDtoAsync(updatedFriendship);
    }

    public async Task<bool> DeleteFriendAsync(Guid userId, Guid friendshipId)
    {
        var friendship = await _friendshipRepository.GetFriendshipAsync(userId, friendshipId);
        if (friendship == null)
        {
            throw new InvalidOperationException("Friendship not found");
        }

        return await _friendshipRepository.DeleteAsync(friendshipId);
    }

    private async Task<FriendshipDto> MapToDtoAsync(Friendship friendship)
    {
        var friend = await _userRepository.GetByIdAsync(friendship.FriendId);
        return new FriendshipDto
        {
            Id = friendship.Id,
            UserId = friendship.UserId,
            FriendId = friendship.FriendId,
            Status = friendship.Status,
            CreatedAt = friendship.CreatedAt,
            UpdatedAt = friendship.UpdatedAt,
            Remark = friendship.Remark,
            Group = friendship.Group,
            IsPinned = friendship.IsPinned,
            IsMuted = friendship.IsMuted,
            Friend = friend != null ? new UserDto
            {
                Id = friend.Id,
                Username = friend.Username,
                Email = friend.Email,
                Avatar = friend.Avatar
            } : null
        };
    }

    private async Task<IEnumerable<FriendshipDto>> MapToDtosAsync(IEnumerable<Friendship> friendships)
    {
        var dtos = new List<FriendshipDto>();
        foreach (var friendship in friendships)
        {
            dtos.Add(await MapToDtoAsync(friendship));
        }
        return dtos;
    }
} 