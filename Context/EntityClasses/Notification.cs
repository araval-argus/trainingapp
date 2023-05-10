using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class Notification
    {
        public int Id { get; set; }
        public int Type { get; set; }
        [ForeignKey("Type")]
        public virtual NotificationType NotificationType { get; set; }
        public int RaisedFor { get; set; }
        [ForeignKey("RaisedFor")]
        public virtual Profile NotificationReceiver { get; set; }
        public int RaisedBy { get; set; }
        [ForeignKey("RaisedBy")]
        public virtual Profile NotificationCreator { get; set; }
        public int? RaisedInGroup { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
