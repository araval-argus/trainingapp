using System;

namespace ChatApp.Models
{
	public class GroupLoadModel
	{
		public int Id { get; set; }
		public string GroupName { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string CreatedBy { get; set; }
		public string? ImagePath { get; set; }
		public string? Description { get; set; }
	}
}
