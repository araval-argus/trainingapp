using Microsoft.AspNetCore.Http;

namespace ChatApp.Models.Group
{
    public class CreateGroupModel
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
