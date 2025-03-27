using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Features.Messages.Commands;
using ChatApp.Application.Features.Messages.Queries;
using ChatApp.API.Hubs;
using ChatApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Controllers;

/// <summary>
/// 消息控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageController(IMediator mediator, IHubContext<ChatHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="message">消息数据</param>
    /// <returns>发送的消息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto message)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? throw new InvalidOperationException("User ID not found in token"));
        
        var command = new SendMessageCommand
        {
            SenderId = userId,
            Message = message
        };

        var result = await _mediator.Send(command);

        // 通过 SignalR 通知接收者
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", result);

        return Ok(result);
    }

    /// <summary>
    /// 获取与指定用户的聊天记录
    /// </summary>
    /// <param name="targetId">目标用户ID</param>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>消息列表</returns>
    [HttpGet("chat/{targetId}")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetChatMessages(
        Guid targetId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? throw new InvalidOperationException("User ID not found in token"));
        
        var query = new GetMessagesQuery
        {
            UserId = userId,
            TargetId = targetId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 更新消息状态
    /// </summary>
    /// <param name="messageId">消息ID</param>
    /// <param name="status">新状态</param>
    /// <returns>更新后的消息</returns>
    [HttpPut("{messageId}/status")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDto>> UpdateMessageStatus(
        Guid messageId,
        [FromBody] MessageStatus status)
    {
        var userId = Guid.Parse(User.Identity?.Name ?? throw new InvalidOperationException("User ID not found in token"));
        
        var command = new UpdateMessageStatusCommand
        {
            MessageId = messageId,
            UserId = userId,
            Status = status
        };

        try
        {
            var result = await _mediator.Send(command);

            // 通过 SignalR 通知发送者消息状态已更新
            await _hubContext.Clients.All.SendAsync("MessageStatusUpdated", messageId, status);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
} 