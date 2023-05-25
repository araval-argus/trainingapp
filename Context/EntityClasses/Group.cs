using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? GroupIcon { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual Profile Creator { get; set; } 

        public DateTime LastUpdatedAt { get; set; } 

        public string? LastUpdatedBy { get; set; }

        [ForeignKey("LastUpdatedBy")]
        public virtual Profile LastModifier { get; set; }   
        
        public bool IsActive { get; set; }
    }
}
