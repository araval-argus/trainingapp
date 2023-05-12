using Microsoft.AspNetCore.Http;

namespace ChatApp.Models.Group
{
    public class GMessageInModel
    {
        public string Content { get; set; }
        public int GroupId { get; set; }
        public string MessageFrom { get; set; }
        public IFormFile File { get; set; }
        public int? RepliedTo { get; set; }
    }
}
