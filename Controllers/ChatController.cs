using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

namespace ChatApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ChatController : ControllerBase
	{
		
		private readonly IChatService _chatService;
		public ChatController(IChatService chatService )
		{
			_chatService = chatService;
		}

		[HttpGet("{Search}")]
		public IActionResult Search(string Search, [FromHeader]string Authorization )
		{
			string userName = _chatService.GetUsername(Authorization);
			return Ok( _chatService.SearchColleague(Search , userName));
		}

		[HttpGet("RecentChat")]
		public IActionResult RecentChat([FromHeader] string Authorization)
		{
			string userName = GetUsernameFromToken(Authorization);

			IEnumerable<RecentChatModel> recentChats =  _chatService.GetRecentUsers(userName);
			if (recentChats != null)
			{
				return Ok(recentChats);
			}
			return BadRequest();
		}

		[HttpGet("MsgList{selUserUserName}")]
		public IActionResult GetMessage(string selUserUserName , [FromHeader] string Authorization)
		{
			Console.WriteLine("1");
			string userName = GetUsernameFromToken(Authorization);

			IEnumerable<MessageDTO> msgList = _chatService.GetMsg(userName, selUserUserName);
			if(msgList!=null)
			{
				return Ok(msgList);
			}
			return Ok();
		}

		[HttpPost("MarkAsRead{selUserUserName}")]
		public IActionResult MarkAsRead(string selUserUserName, [FromHeader] string Authorization)
		{
			string userName = GetUsernameFromToken(Authorization);

			if (userName != null && selUserUserName != null)
			{
				_chatService.MarkAsRead(userName, selUserUserName);
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("SendFileMessage")]
		public IActionResult SaveFileMessage([FromForm] MessageModel msg)
		{
			string filetype = msg.File.ContentType.Split('/')[0];
			if (filetype == "audio" || filetype == "image" || filetype == "video")
			{
				_chatService.SendFileMessage(msg);
				return Ok();
			}
			return BadRequest();
		}

		private string GetUsernameFromToken(string Authorization)
		{
			var handler = new JwtSecurityTokenHandler();
			string auth = Authorization.Split(' ')[1];
			var decodedToken = handler.ReadJwtToken(auth);

			string userName = decodedToken.Claims.First(claim => claim.Type == "sub").Value;
			return userName;
		}
	}
}
