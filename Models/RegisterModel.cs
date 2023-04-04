using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class RegisterModel
    {
        //Added attribute to validate input && Specially Email
        //Also We can add RegularExpressionAttribute to validate password strength

        [Required(ErrorMessage = "Enter First Name")]
        [StringLength(20, MinimumLength =3, ErrorMessage ="First Name Should Be between length of 3-50")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Last Name Should Be between length of 3-50")]

        public string LastName { get; set; }
        [Required(ErrorMessage = "Enter UserName")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "UserName Should Be between length of 3-50")]

        public string UserName { get; set; }
        [Required(ErrorMessage = "Enter Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Email Should Be between length of 3-50")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password Should Be between length of 3-50")]
        public string Password { get; set; }
    }
}
