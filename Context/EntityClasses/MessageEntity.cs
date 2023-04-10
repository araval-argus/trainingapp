using System;

namespace ChatApp.Context.EntityClasses
{
    public class MessageEntity
    {
       
        public int Id { get; set; }
        public string  Message{ get; set; }

        public int SenderID { get; set; }

        public int RecieverID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsSeen { get; set; } = false;
    }
}
