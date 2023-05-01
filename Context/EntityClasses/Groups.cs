using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace ChatApp.Context.EntityClasses
{
	public class Groups
	{
		public int Id { get; set; }
        public string GroupName { get; set; }
		public DateTime? CreatedAt { get; set; }
		public int CreatedBy { get; set; }
		public string? ImagePath { get; set; }
		public string? Description { get; set; }
	}
}
