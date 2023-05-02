using System;

namespace ChatApp.Models
{
	public class NotificationsDTO
	{
		public int Id { get; set; }
		public string MsgFrom { get; set; }
		public string MsgTo { get; set; }
		public int? GroupId { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public string FromImage { get; set; }
	}
}
