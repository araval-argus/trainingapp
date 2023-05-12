using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.User
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [RegularExpression("^([a-z0-9]+@[a-z]+\\.[a-z]{2,3})$", ErrorMessage = "Enter a Valid Email")]
        public string Email { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must contain 8 letter or greater")]
        public string Password { get; set; }
    }
}
