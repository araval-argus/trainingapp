using ChatApp.Business.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class GroupMessageModel
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string SenderUserName { get; set; }

        [Required]
        public int GroupId { get; set; }

        public MessageType MessageType { get; set; } = MessageType.Text;

        public DateTime CreatedAt { get; set; }

        public string? RepliedToMsg { get; set; }
    }
}
