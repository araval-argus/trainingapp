using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class GroupModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? GroupIconUrl { get; set; }
        public IFormFile? GroupIcon { get; set; }
        public DateTime? CreatedAt { get; set; }
        [Required]
        public string CreatorUserName { get; set; }
        public DateTime lastUpdatedAt { get; set; }
        public string? lastUpdatedBy { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTimeStamp { get; set; }
        public bool? LoggedInUserIsAdmin { get; set; }
    }
}
