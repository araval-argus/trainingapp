using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class GroupFileMessageModel
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string SenderUserName { get; set; }
        [Required]
        public int GroupId { get; set; }

    }
}
