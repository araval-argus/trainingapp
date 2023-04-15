using System;

namespace ChatApp.Models
{
	public class RecentChatModel
	{
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public string FirstName { get; set; }
		public string? LastName { get; set; }
		public string ImagePath { get; set; }
		public string UserName { get; set; }
		public int Seen { get; set; }
		public string? Type { get; set; }
	}
}
