using System;

namespace ChatApp.Context.EntityClasses
{
    public class Chat
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string content { get; set; }
        public DateTime sentAt { get; set; }
    }
}
