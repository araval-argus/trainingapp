using System;

namespace ChatApp.Models
{
	public class MessageDTO
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public string  MessageFrom { get; set; }
		public string MessageTo { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string RepliedTo { get; set; }
		public int Seen { get; set; }
		public string? Type { get; set; }
	}
}
