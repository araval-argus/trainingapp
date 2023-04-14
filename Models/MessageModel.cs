using System;

namespace ChatApp.Models
{
	public class MessageModel
	{
		public string Content { get; set; }
		public string MessageFrom { get; set; }
		public string MessageTo { get; set; }
		public DateTime CreatedAt { get; set; }
		public int RepliedTo { get; set; }
		public int Seen { get; set; }
	}
}
