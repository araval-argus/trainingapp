using ChatApp.Business.Helpers;
using System;

namespace ChatApp.Models
{
	public class ColleagueModel
	{
		public string FirstName { get; set; }
		public string? LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string? ImagePath { get; set; }
		public string Designation { get; set; }
	}
}
