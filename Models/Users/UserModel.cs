using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class UserModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public DateTime? CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public int? LastUpdatedBy { get; set; }
        
#nullable enable
        public string? ProfileUrl { get; set; }
    }
}
