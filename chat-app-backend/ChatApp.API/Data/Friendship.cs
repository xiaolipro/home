using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Data
{
    public class Friendship
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid FriendId { get; set; }

        public FriendshipStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // 导航属性
        public virtual User User { get; set; }
        public virtual User Friend { get; set; }
    }

    public enum FriendshipStatus
    {
        Pending = 0,    // 待确认
        Accepted = 1,   // 已接受
        Rejected = 2    // 已拒绝
    }
} 