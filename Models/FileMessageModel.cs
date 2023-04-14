using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class FileMessageModel
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Reciever { get; set; }

        
    }
}
