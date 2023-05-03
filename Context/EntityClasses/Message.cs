using ChatApp.Business.Helpers;
using System;

namespace ChatApp.Context.EntityClasses
{
	public class Message
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public int MessageFrom { get; set; }
		public int MessageTo { get; set; }
		public DateTime CreatedAt { get; set; }
		public int RepliedTo { get; set; }
		public int Seen { get; set; }
		public string Type { get; set; }
	}
}
