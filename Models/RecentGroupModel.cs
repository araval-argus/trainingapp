using System;

namespace ChatApp.Models
{
	public class RecentGroupModel
	{
		public int Id { get; set; }
		public string GroupName { get; set; }
		public string? GroupImage { get; set; }
		public string? LastMsg { get; set; }
		public DateTime? LastMsgAt { get; set; }
		public string? Type { get; set; }
	}
}
