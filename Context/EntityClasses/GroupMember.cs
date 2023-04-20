using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class GroupMember
    {
        public int Id { get; set; }
        public DateTime AddedDate { get; set; }
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Groups Group { get; set; }
        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public Profile Profile { get; set; }
    }
}
