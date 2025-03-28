using ChatApp.API.Data;

namespace ChatApp.API.Models.DTOs
{
    public class FriendshipDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDto User { get; set; }
        public UserDto Friend { get; set; }
    }
} 