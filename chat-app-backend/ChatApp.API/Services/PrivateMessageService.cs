using ChatApp.API.Data;
using ChatApp.API.Exceptions;
using ChatApp.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Services
{
    public class PrivateMessageService
    {
        private readonly ApplicationDbContext _context;

        public PrivateMessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatSessionDto>> GetChatSessionsAsync(Guid userId)
        {
            var sessions = await _context.PrivateMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new ChatSessionDto
                {
                    Id = g.Key,
                    UserId = g.Key,
                    Username = g.Key == userId ? g.First().Sender.Username : g.First().Receiver.Username,
                    Avatar = g.Key == userId ? g.First().Sender.Avatar : g.First().Receiver.Avatar,
                    LastMessage = g.OrderByDescending(m => m.CreatedAt).First().Content,
                    LastMessageTime = g.OrderByDescending(m => m.CreatedAt).First().CreatedAt,
                    UnreadCount = g.Count(m => !m.IsRead && m.ReceiverId == userId)
                })
                .OrderByDescending(s => s.LastMessageTime)
                .ToListAsync();

            return sessions;
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid userId, Guid otherUserId)
        {
            var messages = await _context.PrivateMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => 
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsRead = m.IsRead,
                    Sender = new UserDto
                    {
                        Id = m.Sender.Id,
                        Username = m.Sender.Username,
                        Email = m.Sender.Email,
                        Avatar = m.Sender.Avatar,
                        CreatedAt = m.Sender.CreatedAt,
                        LastLoginAt = m.Sender.LastLoginAt
                    },
                    Receiver = new UserDto
                    {
                        Id = m.Receiver.Id,
                        Username = m.Receiver.Username,
                        Email = m.Receiver.Email,
                        Avatar = m.Receiver.Avatar,
                        CreatedAt = m.Receiver.CreatedAt,
                        LastLoginAt = m.Receiver.LastLoginAt
                    }
                })
                .ToListAsync();

            return messages;
        }

        public async Task<MessageDto> SendMessageAsync(Guid senderId, Guid receiverId, string content)
        {
            var message = new PrivateMessage
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.PrivateMessages.Add(message);
            await _context.SaveChangesAsync();

            var messageDto = await _context.PrivateMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.Id == message.Id)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsRead = m.IsRead,
                    Sender = new UserDto
                    {
                        Id = m.Sender.Id,
                        Username = m.Sender.Username,
                        Email = m.Sender.Email,
                        Avatar = m.Sender.Avatar,
                        CreatedAt = m.Sender.CreatedAt,
                        LastLoginAt = m.Sender.LastLoginAt
                    },
                    Receiver = new UserDto
                    {
                        Id = m.Receiver.Id,
                        Username = m.Receiver.Username,
                        Email = m.Receiver.Email,
                        Avatar = m.Receiver.Avatar,
                        CreatedAt = m.Receiver.CreatedAt,
                        LastLoginAt = m.Receiver.LastLoginAt
                    }
                })
                .FirstOrDefaultAsync();

            return messageDto;
        }

        public async Task MarkAsReadAsync(Guid messageId)
        {
            var message = await _context.PrivateMessages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.PrivateMessages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }

        /// <summary>
        /// 编辑消息
        /// </summary>
        public async Task<MessageDto> EditMessageAsync(Guid messageId, Guid currentUserId, string newContent)
        {
            var message = await _context.PrivateMessages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
                throw new BusinessException("NotFound", "消息不存在");

            if (message.SenderId != currentUserId)
                throw new BusinessException("Forbidden", "只能编辑自己发送的消息");

            if (message.IsRecalled)
                throw new BusinessException("InvalidOperation", "已撤回的消息不能编辑");

            // 检查消息发送时间是否超过5分钟
            if (DateTime.UtcNow - message.CreatedAt > TimeSpan.FromMinutes(5))
                throw new BusinessException("InvalidOperation", "只能编辑5分钟内发送的消息");

            message.Content = newContent;
            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt,
                IsRead = message.IsRead,
                IsRecalled = message.IsRecalled,
                Sender = new UserDto
                {
                    Id = message.Sender.Id,
                    Username = message.Sender.Username,
                    Avatar = message.Sender.Avatar
                }
            };
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        public async Task RecallMessageAsync(Guid messageId, Guid currentUserId)
        {
            var message = await _context.PrivateMessages
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
                throw new BusinessException("NotFound", "消息不存在");

            if (message.SenderId != currentUserId)
                throw new BusinessException("Forbidden", "只能撤回自己发送的消息");

            if (message.IsRecalled)
                throw new BusinessException("InvalidOperation", "消息已经被撤回");

            // 检查消息发送时间是否超过5分钟
            if (DateTime.UtcNow - message.CreatedAt > TimeSpan.FromMinutes(5))
                throw new BusinessException("InvalidOperation", "只能撤回5分钟内发送的消息");

            message.IsRecalled = true;
            message.Content = "[消息已撤回]";
            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
} 