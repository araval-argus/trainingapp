using System;

namespace ChatApp.Models    
{
    public class SentGroupMessage
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int GroupId { get; set; }
        public string SenderUserName { get; set; }
        public DateTime sentAt { get; set; }
        public int? ReplyingMessageId { get; set; }
        public string Type { get; set; }
        public string ReplyingContent { get; set; }
    }
}
