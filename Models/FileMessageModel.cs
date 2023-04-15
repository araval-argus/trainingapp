using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class FileMessageModel
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string SenderUserName { get; set; }

        [Required]
        public string RecieverUserName { get; set; }

        
    }
}
