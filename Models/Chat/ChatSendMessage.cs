using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.Chat
{
    public class ChatSendMessage
    {
        [Required]
        public string Sender { get; set; }

        [Required]
        public string Receiver { get; set; }

        [Required]
        public string Type { get; set; }

#nullable enable
        public string? Content { get; set; }
#nullable enable
        public IFormFile? Image{ get; set; }

        public int? ReplyTo { get; set; }

    }
}
