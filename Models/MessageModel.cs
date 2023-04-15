using ChatApp.Business.Helpers;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class MessageModel
    {

        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string SenderUserName { get; set; }

        [Required]
        public string RecieverUserName { get; set; }

        public string? RepliedToMsg { get; set; }

        public MessageType MessageType { get; set; } = Business.Helpers.MessageType.Text;

        public DateTime? CreatedAt { get; set; }

        public bool? IsSeen { get; set; }

    }
}
