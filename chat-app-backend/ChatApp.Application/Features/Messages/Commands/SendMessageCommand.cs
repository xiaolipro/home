using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;
using MediatR;

namespace ChatApp.Application.Features.Messages.Commands;

/// <summary>
/// 发送消息命令
/// </summary>
public class SendMessageCommand : IRequest<MessageDto>
{
    /// <summary>
    /// 发送者ID
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// 消息数据
    /// </summary>
    public SendMessageDto Message { get; set; } = null!;
}

/// <summary>
/// 发送消息命令处理器
/// </summary>
public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;

    public SendMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            SenderId = request.SenderId,
            ReceiverId = request.Message.ReceiverId,
            Type = request.Message.Type,
            Content = request.Message.Content,
            Metadata = request.Message.Metadata
        };

        var savedMessage = await _messageRepository.AddAsync(message);

        return new MessageDto
        {
            Id = savedMessage.Id,
            SenderId = savedMessage.SenderId,
            ReceiverId = savedMessage.ReceiverId,
            Type = savedMessage.Type,
            Content = savedMessage.Content,
            Status = savedMessage.Status,
            SentAt = savedMessage.SentAt,
            ReadAt = savedMessage.ReadAt,
            Metadata = savedMessage.Metadata
        };
    }
} 