using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class GeneralMessages
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Desc { get; set; }
        public int PayPerHour { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("MessageFromUser")]
        public int MessageFrom { get; set; }
        public virtual Profile MessageFromUser { get; set; }

        [ForeignKey("AreaFK")]
        public int AreaId { get; set; }
        public virtual Areas AreaFK { get; set; }
    }
}
