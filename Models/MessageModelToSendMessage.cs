using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class MessageModelToSendMessage
    {
        [Required]
        public string Message { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Reciever { get; set; }
    }
}
