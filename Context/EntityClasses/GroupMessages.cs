using System;

namespace ChatApp.Context.EntityClasses
{
	public class GroupMessages
	{
		public int Id { get; set; }
		public int	GrpId { get; set; }
		public string Content { get; set; }
		public int MessageFrom { get; set; }
		public DateTime CreatedAt { get; set; }
		public int? RepliedTo { get; set; }
		public string? Type { get; set; }
	}
}
