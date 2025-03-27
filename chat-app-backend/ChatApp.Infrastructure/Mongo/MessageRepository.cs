using ChatApp.Domain.Entities;
using ChatApp.Domain.Repositories;
using ChatApp.Infrastructure.Repositories;

namespace ChatApp.Infrastructure.Mongo;

/// <summary>
/// MongoDB消息仓储实现
/// </summary>
public class MessageRepository : MongoBaseRepository<BaseMessage, Guid>, IMessageRepository
{
   
} 