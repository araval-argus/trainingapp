using System;

namespace ChatApp.Models
{
    public class ChatModel
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string content { get; set; }
        public DateTime sent { get; set; }
    }
}
