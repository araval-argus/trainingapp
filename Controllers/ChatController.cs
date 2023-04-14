using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

namespace ChatApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
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
			string username = _chatService.GetUsername(Authorization);
			return Ok( _chatService.SearchColleague(Search , username));
		}

		[HttpPost("Message")]
		public IActionResult DoMessage(MessageModel message)
		{
			if (message!=null) 
			{
				return Ok(_chatService.DoMessage(message));
			}
			return BadRequest();
		}

		[HttpGet("RecentChat")]
		public IActionResult RecentChat([FromHeader] string Authorization)
		{
			var handler = new JwtSecurityTokenHandler();
			string auth = Authorization.Split(' ')[1];
			var decodedToken = handler.ReadJwtToken(auth);

			string username = decodedToken.Claims.First(claim => claim.Type == "sub").Value;

			IEnumerable<RecentChatModel> recentChats =  _chatService.GetRecentUsers(username);
			if (recentChats != null)
			{
				return Ok(recentChats);
			}
			return BadRequest();
		}

		[HttpGet("MsgList{seluserusername}")]
		public IActionResult GetMessage(string seluserusername , [FromHeader] string Authorization)
		{
			var handler = new JwtSecurityTokenHandler();
			string auth = Authorization.Split(' ')[1];
			var decodedToken = handler.ReadJwtToken(auth);

			string username = decodedToken.Claims.First(claim => claim.Type == "sub").Value;

			IEnumerable<MessageSendModel> msgList = _chatService.GetMsg(username, seluserusername);
			if(msgList!=null)
			{
				return Ok(msgList);
			}
			return Ok();
		}

		[HttpPost("MarkAsRead{seluserusername}")]
		public IActionResult MarkAsRead(string seluserusername ,[FromHeader] string Authorization)
		{
			var handler = new JwtSecurityTokenHandler();
			string auth = Authorization.Split(' ')[1];
			var decodedToken = handler.ReadJwtToken(auth);

			string username = decodedToken.Claims.First(claim => claim.Type == "sub").Value;
			if (username != null && seluserusername != null)
			{
				_chatService.MarkAsRead(username, seluserusername);
				return Ok();
			}
			return BadRequest();
		}
	}
}
