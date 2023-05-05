using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupIcon { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual Profile Creator { get; set; } 

        public DateTime LastUpdatedAt { get; set; } 

        public int LastUpdatedBy { get; set; }
        [ForeignKey("LastUpdatedBy")]
        public virtual Profile LastModifier { get; set; }

        public bool IsActive { get; set; }
    }
}
