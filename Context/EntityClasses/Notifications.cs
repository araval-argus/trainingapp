using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class Notifications
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public int User { get; set; }
        [ForeignKey("User")]
        public Profile Profile { get; set; }
        public int isSeen { get; set; }
        public int isGroup { get; set; }
    }
}
