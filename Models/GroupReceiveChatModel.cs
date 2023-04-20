using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class GroupReceiveChatModel
    {
        public string Content { get; set; }
        public int GroupId { get; set; }
        public int RepliedId { get; set; }
    }
}
