using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.User
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must contain 8 letter or greater")]
        public string Password { get; set; }
    }
}
