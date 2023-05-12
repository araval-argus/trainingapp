using System;

namespace ChatApp.Context.EntityClasses
{
	public class GroupMembers
	{
		public int Id { get; set; }
		public int ProfileId { get; set; }
		public int GrpId { get; set; }
		public DateTime JoinedAt { get; set; }
		public int Admin { get; set; }
	}
}
