using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models.MessageModel
{
    public class TextMessageModel
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }
        
        public DateTime? DateTime { get; set; }

        public bool? IsSeen { get; set; } = false;

        public bool? IsReply { get; set; } = false;

        public int? ReplyedToId { get; set; } = 0;

        public string Type { get; set; } 
    }
}
