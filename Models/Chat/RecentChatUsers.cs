using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.Chat
{
    public class RecentChatUsers
    {
        public int UnreadCount { get; set; }

        public string LastMessage { get; set; }
        public string LastMessageType { get; set; }

        public UserModel User { get; set; }
    }
}
