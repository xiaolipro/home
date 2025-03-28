using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Data
{
    public class PrivateMessage
    {
        public Guid Id { get; set; }

        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        // 导航属性
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
} 