using ChatApp.Business.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class GroupMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string SenderID { get; set; }

        [ForeignKey("SenderID")]
        public virtual Profile Sender { get; set; }

        [Required]
        public int GroupID { get; set; }
        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }

        public MessageType MessageType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int RepliedToMsg { get; set; }
    }
}
