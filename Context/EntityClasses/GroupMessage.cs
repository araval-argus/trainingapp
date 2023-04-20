using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class GroupMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public Profile SenderProfile { get; set; }
        public int GroupID { get; set; }
        [ForeignKey("GroupID")]
        public Groups Group { get; set; }
        public string Content { get; set; }

        public int? ReplyMessageID { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }

    }
}
