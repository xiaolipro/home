using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Repositories;
using MediatR;

namespace ChatApp.Application.Features.Messages.Queries;

/// <summary>
/// 获取消息查询
/// </summary>
public class GetMessagesQuery : IRequest<IEnumerable<MessageDto>>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 目标ID（用户ID或群组ID）
    /// </summary>
    public Guid TargetId { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }
}

/// <summary>
/// 获取消息查询处理器
/// </summary>
public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;

    public GetMessagesQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetMessagesBetweenUsersAsync(
            request.UserId,
            request.TargetId,
            request.Page,
            request.PageSize);

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Type = m.Type,
            Content = m.Content,
            Status = m.Status,
            SentAt = m.SentAt,
            ReadAt = m.ReadAt,
            Metadata = m.Metadata
        });
    }
} 