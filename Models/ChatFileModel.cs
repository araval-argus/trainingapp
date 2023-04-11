using Microsoft.AspNetCore.Http;

namespace ChatApp.Models
{
    public class ChatFileModel
    {
        public string to { get; set; }
        public IFormFile file { get; set; }
    }
}
