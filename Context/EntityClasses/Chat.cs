using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Context.EntityClasses
{
    public class Chat
    {
        public int Id { get; set; }

        public int MessageFrom { get; set; }

        public int MessageTo { get; set; }

        public string Type { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt{ get; set; }

        //[ForeignKey("MessageFrom")]
        // public virtual Profile MessageFromUser { get; set; }

        //[ForeignKey("MessageTo")]
        //public virtual Profile MessageToUser { get; set; }
    }
}
