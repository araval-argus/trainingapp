using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class RegisterModel
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
