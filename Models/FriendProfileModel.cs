using System;

namespace ChatApp.Models
{
    public class FriendProfileModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }   

        public string Email { get; set; }

        public string ImageUrl { get; set; }
        public string Designation { get; set; }

        public string? LastMessage { get; set; } = null;

        public DateTime? LastMessageTimeStamp { get; set; } = null;

        public int UnreadMessageCount { get; set; }
    }
}
