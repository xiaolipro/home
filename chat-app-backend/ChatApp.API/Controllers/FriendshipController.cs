using ChatApp.Application.DTOs.Friendship;
using ChatApp.Application.Services;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

/// <summary>
/// 好友关系控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FriendshipController : ControllerBase
{
    private readonly IFriendshipService _friendshipService;

    public FriendshipController(IFriendshipService friendshipService)
    {
        _friendshipService = friendshipService;
    }

    /// <summary>
    /// 获取好友列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetFriends()
    {
        var userId = GetUserId();
        var friends = await _friendshipService.GetFriendsAsync(userId);
        return Ok(friends);
    }

    /// <summary>
    /// 获取待确认的好友请求
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetPendingRequests()
    {
        var userId = GetUserId();
        var requests = await _friendshipService.GetPendingRequestsAsync(userId);
        return Ok(requests);
    }

    /// <summary>
    /// 发送好友请求
    /// </summary>
    [HttpPost("request")]
    public async Task<ActionResult<FriendshipDto>> SendFriendRequest([FromBody] AddFriendRequestDto request)
    {
        var userId = GetUserId();
        var friendship = await _friendshipService.SendFriendRequestAsync(userId, request);
        return Ok(friendship);
    }

    /// <summary>
    /// 接受好友请求
    /// </summary>
    [HttpPost("{friendshipId}/accept")]
    public async Task<ActionResult<FriendshipDto>> AcceptFriendRequest(Guid friendshipId)
    {
        var userId = GetUserId();
        var friendship = await _friendshipService.HandleFriendRequestAsync(userId, friendshipId, FriendshipStatus.Accepted);
        return Ok(friendship);
    }

    /// <summary>
    /// 拒绝好友请求
    /// </summary>
    [HttpPost("{friendshipId}/reject")]
    public async Task<ActionResult<FriendshipDto>> RejectFriendRequest(Guid friendshipId)
    {
        var userId = GetUserId();
        var friendship = await _friendshipService.HandleFriendRequestAsync(userId, friendshipId, FriendshipStatus.Rejected);
        return Ok(friendship);
    }

    /// <summary>
    /// 更新好友关系
    /// </summary>
    [HttpPut("{friendshipId}")]
    public async Task<ActionResult<FriendshipDto>> UpdateFriendship(
        Guid friendshipId,
        [FromBody] UpdateFriendshipDto request)
    {
        var userId = GetUserId();
        var friendship = await _friendshipService.UpdateFriendshipAsync(
            userId,
            friendshipId,
            request.Remark,
            request.Group,
            request.IsPinned,
            request.IsMuted);
        return Ok(friendship);
    }

    /// <summary>
    /// 删除好友
    /// </summary>
    [HttpDelete("{friendshipId}")]
    public async Task<ActionResult> DeleteFriend(Guid friendshipId)
    {
        var userId = GetUserId();
        var result = await _friendshipService.DeleteFriendAsync(userId, friendshipId);
        return result ? Ok() : NotFound();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new InvalidOperationException("User ID not found in token");
        }
        return userId;
    }
} 