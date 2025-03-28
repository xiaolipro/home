using ChatApp.API.Models.DTOs;
using ChatApp.API.Models.Requests;
using ChatApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FriendshipsController : ControllerBase
    {
        private readonly FriendshipService _friendshipService;

        public FriendshipsController(FriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        /// <summary>
        /// 获取当前用户的好友列表
        /// </summary>
        /// <returns>好友列表</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FriendshipDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetFriends()
        {
            var userId = GetCurrentUserId();
            var friends = await _friendshipService.GetFriendsAsync(userId);
            return Ok(friends);
        }

        /// <summary>
        /// 获取待处理的好友请求
        /// </summary>
        /// <returns>待处理的好友请求列表</returns>
        [HttpGet("requests")]
        [ProducesResponseType(typeof(IEnumerable<FriendshipDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetFriendRequests()
        {
            var userId = GetCurrentUserId();
            var requests = await _friendshipService.GetFriendRequestsAsync(userId);
            return Ok(requests);
        }

        /// <summary>
        /// 发送好友请求
        /// </summary>
        /// <param name="request">发送好友请求</param>
        /// <returns>创建的好友关系</returns>
        [HttpPost("requests")]
        [ProducesResponseType(typeof(FriendshipDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FriendshipDto>> SendFriendRequest([FromBody] SendFriendRequestRequest request)
        {
            var userId = GetCurrentUserId();
            var friendship = await _friendshipService.SendFriendRequestAsync(userId, request.FriendId);
            return CreatedAtAction(nameof(GetFriends), new { }, friendship);
        }

        /// <summary>
        /// 接受好友请求
        /// </summary>
        /// <param name="requestId">好友请求ID</param>
        /// <returns>操作结果</returns>
        [HttpPut("requests/{requestId}/accept")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
        {
            var userId = GetCurrentUserId();
            await _friendshipService.AcceptFriendRequestAsync(userId, requestId);
            return NoContent();
        }

        /// <summary>
        /// 拒绝好友请求
        /// </summary>
        /// <param name="requestId">好友请求ID</param>
        /// <returns>操作结果</returns>
        [HttpPut("requests/{requestId}/reject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectFriendRequest(Guid requestId)
        {
            var userId = GetCurrentUserId();
            await _friendshipService.RejectFriendRequestAsync(userId, requestId);
            return NoContent();
        }

        /// <summary>
        /// 删除好友关系
        /// </summary>
        /// <param name="friendshipId">好友关系ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{friendshipId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFriendship(Guid friendshipId)
        {
            var userId = GetCurrentUserId();
            await _friendshipService.DeleteFriendshipAsync(userId, friendshipId);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new UnauthorizedAccessException("无效的用户认证");
            }
            return userId;
        }
    }
} 