using ChatApp.Business.Helpers;
using System;

namespace ChatApp.Context.EntityClasses
{
    public class MessageEntity
    {
       
        public int Id { get; set; }
        public string  Message{ get; set; }

        public int SenderID { get; set; }

        public int RecieverID { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsSeen { get; set; } = false;

        public int RepliedToMsg { get; set; } = -1;

        public MessageType MessageType { get; set; }
    }
}
