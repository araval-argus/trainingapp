using System;

namespace ChatApp.Context.EntityClasses
{
    public class Chat
    {
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string content { get; set; }
        public DateTime sentAt { get; set; }
        public int replyToChat { get; set; }
    }
}
