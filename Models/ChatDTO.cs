using System;
using System.Collections.Generic;

namespace ChatApp.Models
{
    public class ChatDTO
    {
        public bool IsSent { get; set; }
        public List<chatFormat> chatList { get; set; }
    }

    public class chatFormat
    {
        public int id { get; set; }
        public string content { get; set; }
        public DateTime sentAt { get; set; }
        public int replyToChat { get; set; }
    }

    public class recentChatDTO
    {
        public profileDTO to { get; set; }
        public chatFormat chatContent { get; set; }
    }
}
