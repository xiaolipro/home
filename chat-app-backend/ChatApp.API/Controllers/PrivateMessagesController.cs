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
    public class PrivateMessagesController : ControllerBase
    {
        private readonly PrivateMessageService _messageService;

        public PrivateMessagesController(PrivateMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// 获取当前用户的所有聊天会话
        /// </summary>
        /// <returns>聊天会话列表</returns>
        [HttpGet("sessions")]
        [ProducesResponseType(typeof(IEnumerable<ChatSessionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChatSessionDto>>> GetChatSessions()
        {
            var userId = GetCurrentUserId();
            var sessions = await _messageService.GetChatSessionsAsync(userId);
            return Ok(sessions);
        }

        /// <summary>
        /// 获取与指定用户的聊天记录
        /// </summary>
        /// <param name="userId">目标用户ID</param>
        /// <returns>聊天记录列表</returns>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            var messages = await _messageService.GetMessagesAsync(currentUserId, userId);
            return Ok(messages);
        }

        /// <summary>
        /// 发送私信
        /// </summary>
        /// <param name="request">发送消息请求</param>
        /// <returns>发送的消息</returns>
        [HttpPost]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageRequest request)
        {
            var senderId = GetCurrentUserId();
            var message = await _messageService.SendMessageAsync(senderId, request.ReceiverId, request.Content);
            return CreatedAtAction(nameof(GetMessages), new { userId = request.ReceiverId }, message);
        }

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns>操作结果</returns>
        [HttpPut("{messageId}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(Guid messageId)
        {
            await _messageService.MarkAsReadAsync(messageId);
            return NoContent();
        }

        /// <summary>
        /// 获取未读消息数量
        /// </summary>
        /// <returns>未读消息数量</returns>
        [HttpGet("unread/count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var count = await _messageService.GetUnreadCountAsync(userId);
            return Ok(count);
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