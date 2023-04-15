using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
	public class UpdateModel
	{
		[Required]
		public string FirstName { get; set; }
		public string? LastName { get; set; }
		public string UserName { get; set; }
		[Required]
		[RegularExpression("^([a-z0-9]+@[a-z]+\\.[a-z]{2,3})$", ErrorMessage = "Enter a Valid Email")]
		public string Email { get; set; }
		public string Designation { get; set; }
		public IFormFile? ProfileImage { get; set; }
		public DateTime? LastUpdatedAt { get; set; }
	}
}
