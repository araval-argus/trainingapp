using Microsoft.AspNetCore.Http;

namespace ChatApp.Models
{
    public class GroupReceiveMessageModel
    {
        public IFormFile File { get; set; }
        public int GroupId { get; set; }
        public int RepliedId { get; set; }
        public int Type { get; set; }
    }
}
