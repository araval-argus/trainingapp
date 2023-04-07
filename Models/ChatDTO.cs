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
        public string content { get; set; }
        public DateTime sentAt { get; set; }
    }
}
