using ChatApp.Application.DTOs.Friendship;
using ChatApp.Application.Services;
using ChatApp.Domain.Constants;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Hubs;

/// <summary>
/// 聊天Hub
/// </summary>
public class ChatHub : Hub
{
    private readonly IFriendshipService _friendshipService;
    private readonly IMessageRepository _messageRepository;

    public ChatHub(IFriendshipService friendshipService, IMessageRepository messageRepository)
    {
        _friendshipService = friendshipService;
        _messageRepository = messageRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        await Clients.All.SendAsync(HubCommands.UserOnline, userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        await Clients.All.SendAsync(HubCommands.UserOffline, userId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverId, string content)
    {
        var senderId = GetUserId();
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = Guid.Parse(receiverId),
            Type = MessageType.Text,
            Content = content
        };

        await _messageRepository.AddAsync(message);
        await Clients.User(receiverId).SendAsync(HubCommands.ReceiveMessage, message);
    }

    public async Task MarkMessageAsRead(Guid messageId)
    {
        var userId = GetUserId();
        await _messageRepository.UpdateStatusAsync(messageId, MessageStatus.Read);
        await Clients.All.SendAsync(HubCommands.MessageRead, messageId, userId);
    }

    public async Task RecallMessage(Guid messageId)
    {
        var userId = GetUserId();
        var result = await _messageRepository.RecallMessageAsync(messageId, userId);
        if (result)
        {
            await Clients.All.SendAsync(HubCommands.MessageRecalled, messageId, userId);
        }
    }

    public async Task SendFriendRequest(string friendId)
    {
        var userId = GetUserId();
        var request = new AddFriendRequestDto
        {
            FriendId = Guid.Parse(friendId)
        };

        var friendship = await _friendshipService.SendFriendRequestAsync(userId, request);
        await Clients.User(friendId).SendAsync(HubCommands.FriendRequest, friendship);
    }

    public async Task HandleFriendRequest(Guid friendshipId, FriendshipStatus status)
    {
        var userId = GetUserId();
        var friendship = await _friendshipService.HandleFriendRequestAsync(userId, friendshipId, status);
        await Clients.All.SendAsync(HubCommands.FriendRequestHandled, friendship);
    }

    public async Task UpdateFriendStatus(Guid friendshipId, string remark, string group, bool isPinned, bool isMuted)
    {
        var userId = GetUserId();
        var friendship = await _friendshipService.UpdateFriendshipAsync(
            userId,
            friendshipId,
            remark,
            group,
            isPinned,
            isMuted);
        await Clients.All.SendAsync(HubCommands.FriendStatusChanged, friendship);
    }

    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst("UserId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new InvalidOperationException("User ID not found in token");
        }
        return userId;
    }
} 