using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
        [Required]
        public string TagClasses { get; set; }
        [Required]
        public string TagStyle { get; set; }
    }
}
