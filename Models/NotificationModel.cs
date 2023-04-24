using ChatApp.Context.EntityClasses;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace ChatApp.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public int isSeen { get; set; } = 0;
        public int isGroup { get; set; } = 0;
    }
}
