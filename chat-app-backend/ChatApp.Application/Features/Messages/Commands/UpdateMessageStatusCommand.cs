using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Repositories;
using MediatR;

namespace ChatApp.Application.Features.Messages.Commands;

/// <summary>
/// 更新消息状态命令
/// </summary>
public class UpdateMessageStatusCommand : IRequest<MessageDto>
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// 用户ID（消息接收者）
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 新状态
    /// </summary>
    public MessageStatus Status { get; set; }
}

/// <summary>
/// 更新消息状态命令处理器
/// </summary>
public class UpdateMessageStatusCommandHandler : IRequestHandler<UpdateMessageStatusCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;

    public UpdateMessageStatusCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto> Handle(UpdateMessageStatusCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId);
        if (message == null)
        {
            throw new InvalidOperationException("Message not found");
        }

        if (message.ReceiverId != request.UserId)
        {
            throw new InvalidOperationException("User is not the receiver of this message");
        }

        message.Status = request.Status;
        if (request.Status == MessageStatus.Read)
        {
            message.ReadAt = DateTime.UtcNow;
        }

        await _messageRepository.UpdateAsync(message);
        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Type = message.Type,
            Content = message.Content,
            Status = message.Status,
            SentAt = message.SentAt,
            ReadAt = message.ReadAt,
            Metadata = message.Metadata
        };
    }
} 