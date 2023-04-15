using Microsoft.AspNetCore.Http;
using System;

namespace ChatApp.Models
{
	public class MessageModel
	{
		public string? Content { get; set; }
		public string MessageFrom { get; set; }
		public string MessageTo { get; set; }
		public DateTime? CreatedAt { get; set; }
		public int? RepliedTo { get; set; }
		public int? Seen { get; set; }
		public IFormFile? File { get; set; }
		public string? Type { get; set; }
	}
}
