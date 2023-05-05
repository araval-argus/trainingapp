using ChatApp.Business.Helpers;
using System;

namespace ChatApp.Models
{
    public class profileDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int ProfileType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string imagePath { get; set; }
        public int status { get; set; }
    }
}
