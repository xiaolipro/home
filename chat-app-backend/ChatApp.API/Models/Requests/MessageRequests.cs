using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Models.Requests
{
    public class SendMessageRequest
    {
        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
    }
} 