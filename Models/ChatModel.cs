using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class ChatModel
    {
        [Required(ErrorMessage = "Please Add Receiver")]
        public string To { get; set; }
        [Required(ErrorMessage = "Please Add Content to be sent")]
        public string content { get; set; }
    }
}
