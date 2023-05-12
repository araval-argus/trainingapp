using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models.User
{
    public class ChangePasswordModel
    {
        [Required]
        [MinLength(8)]
        public string Old { get; set; }

        [Required]
        [MinLength(8)]
        public string Newp { get; set; }

        [Required]
        [MinLength(8)]
        public string Verify { get; set; }
    }
}
