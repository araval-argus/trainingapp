using System.Runtime.InteropServices;
using System;

namespace ChatApp.Context.EntityClasses
{
	public class Notifications
	{
		public int Id { get; set; }
		public int FromId { get; set; }
		public int ToId { get; set; }
		public int? GroupId { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
