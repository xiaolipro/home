using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Models.Requests
{
    public class SendFriendRequestRequest
    {
        [Required]
        public Guid RequestId { get; set; }
    }

    public class RespondToFriendRequestRequest
    {
        [Required]
        public Guid RequestId { get; set; }
    }
} 