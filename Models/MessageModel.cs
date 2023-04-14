using ChatApp.Business.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class MessageModel
    {
        [Required]
        public string Message { get; set; }

        [Required]
        public int SenderID { get; set; }

        [Required]
        public int RecieverID { get; set; }


        public int RepliedToMsg { get; set; }

        public MessageType MessageType { get; set; }

    }
}
