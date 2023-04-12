using System;

namespace ChatApp.Models
{
	public class MessageSendModel
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public string  MessageFrom { get; set; }
		public string MessageTo { get; set; }
		public DateTime? CreatedAt { get; set; }
	}
}
