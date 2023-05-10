using ChatApp.Context.EntityClasses;
using System;

namespace ChatApp.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string RaisedFor { get; set; }
        public string RaisedBy { get; set; }
        public string? RaisedInGroup { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
