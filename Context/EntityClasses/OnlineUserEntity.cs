using System.ComponentModel.DataAnnotations;

namespace ChatApp.Context.EntityClasses
{
    public class OnlineUserEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string ConnectionId { get; set; }
    }
}
