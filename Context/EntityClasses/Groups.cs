using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Context.EntityClasses
{
    public class Groups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfileImage { get; set; }
        public int Admin { get; set; }
        [ForeignKey("Admin")]
        [ValidateNever]
        public Profile AdminProfile { get; set; }
    }
}
