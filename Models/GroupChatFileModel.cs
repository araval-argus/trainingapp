using Microsoft.AspNetCore.Http;

namespace ChatApp.Models
{
    public class GroupChatFileModel
    {
        public IFormFile File { get; set; }
        public string GroupId { get; set; }
        public string RepliedId { get; set; }
    }
}
