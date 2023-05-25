using ChatApp.Business.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class MessageEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string  Message{ get; set; }

        [Required]
        public string SenderID { get; set; }

        [Required]
        public string RecieverID { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsSeen { get; set; } = false;

        public int RepliedToMsg { get; set; } = -1;

        public MessageType MessageType { get; set; }
    }
}
