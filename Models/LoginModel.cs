using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class LoginModel
    {
        //Added attribute to validate input && Specially Email
        //Also We can add RegularExpressionAttribute to validate password strength
        [Required(ErrorMessage = "Invalid Credential")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Invalid Credential")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Invalid Credential")]
        public string Password { get; set; }
    }
}
