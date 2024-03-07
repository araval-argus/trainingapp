using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models.GeneralMessages
{
    public class CreatePostModel
    {
            [Required]
            public string Role { get; set; }

            [Required]
            public string Desc { get; set; }

            public int PayPerHour { get; set; }

            public int AreaCode { get; set; }
    }
}
