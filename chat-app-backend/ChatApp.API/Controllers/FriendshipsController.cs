using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatApp.API.Models;
using ChatApp.API.Models.DTOs;
using ChatApp.API.Models.Requests;
using ChatApp.API.Services;

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
        [HttpGet("friends")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetFriends()
        {
            var friends = await _friendshipService.GetFriendsAsync();
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
            var requests = await _friendshipService.GetPendingRequestsAsync();
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
            var friendship = await _friendshipService.SendFriendRequestAsync(request.RequestId);
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
            await _friendshipService.AcceptFriendRequestAsync(requestId);
            return Ok();
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
            await _friendshipService.RejectFriendRequestAsync(requestId);
            return Ok();
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
            await _friendshipService.DeleteFriendshipAsync(friendshipId);
            return NoContent();
        }

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="query">搜索关键词</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>用户列表</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<UserDto>>> SearchUsers([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("搜索关键词不能为空");
            }

            var result = await _friendshipService.SearchUsersAsync(query, page, pageSize);
            return Ok(result);
        }
    }
} 