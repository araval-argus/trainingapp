using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class GroupMember
    {
        public int Id { get; set; }
        public int ProfileID { get; set; }

        [ForeignKey("ProfileID")]
        public virtual Profile Member { get; set; }

        public int GroupID { get; set; }
        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.Now;
    }
}
