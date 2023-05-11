using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Infrastructure.ServiceImplementation;
using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

namespace ChatApp.Controllers
{

	[Route("api/[controller]")]
	[ApiController]

	public class GroupController : ControllerBase
	{

		private readonly IGroupService _groupService;
		private readonly ChatAppContext context;
		public GroupController(IGroupService groupService, ChatAppContext context)
		{
			_groupService = groupService;
			this.context = context;
		}

		[HttpPost("CreateGroup")]
		public IActionResult CreateGroup([FromForm] CreateGroupModel createGroup, [FromHeader] string Authorization)
		{

			if (createGroup.GroupName != null && Authorization != null)
			{
				var username = GetUsernameFromToken(Authorization);
				return Ok(_groupService.CreateGroup(username, createGroup));
			}
			return BadRequest();
		}

		[HttpGet("RecentGroup")]
		public IActionResult RecentChat([FromHeader] string Authorization)
		{
			string userName = GetUsernameFromToken(Authorization);

			IEnumerable<RecentGroupModel> recentGroups = _groupService.GetRecentGroups(userName);
			if (recentGroups != null)
			{
				return Ok(recentGroups);
			}
			return BadRequest();
		}

		[HttpPost("GetGroup")]
		public IActionResult GetGroupDetail([FromBody] int groupId, [FromHeader] string Authorization)
		{
			string username = GetUsernameFromToken(Authorization);
			if (context.Groups.Any(u => u.Id == groupId))
			{
				return Ok(_groupService.getGroup(groupId, username));
			}
			return BadRequest();
		}

		[HttpPost("AllProfiles")]
		public IActionResult GetAllProfiles([FromBody] int groupId, [FromHeader] string Authorization)
		{
			string userName = GetUsernameFromToken(Authorization);
			if (context.Groups.Any(u => u.Id == groupId))
			{
				return Ok(_groupService.getAllProfiles(groupId, userName));
			}
			return BadRequest();
		}

		[HttpPost("GetAllMembers")]
		public IActionResult GetAllMembers([FromBody] int groupId)
		{
			return Ok(_groupService.getAllMembers(groupId));
		}

		[HttpPost("AddMemberToGroup/{grpId}")]
		public IActionResult AddMemberToGroup(int grpId, [FromBody] string[] selUsers, [FromHeader] string Authorization)
		{
			string username = GetUsernameFromToken(Authorization);
			if (selUsers != null)
			{
				return Ok(_groupService.addMembersToGroup(grpId, selUsers, username));
			}
			return BadRequest();
		}

		[HttpPost("UpdateGroup/{grpId}")]
		public IActionResult UpdateGroup(int grpId, [FromForm] CreateGroupModel group, [FromHeader] string Authorization)
		{
			string username = GetUsernameFromToken(Authorization);
			if (group.GroupName != null && username != null)
			{
				return Ok(_groupService.updateGroup(username, group, grpId));
			}
			return BadRequest();
		}

		[HttpPost("LeaveGroup")]
		public IActionResult LeaveGroup([FromBody] int groupId, [FromHeader] string Authorization)
		{
			string username = GetUsernameFromToken(Authorization);
			if (groupId != null)
			{
				_groupService.leaveGroup(username, groupId);
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("MakeAdmin/{groupId}")]
		public IActionResult MakeAdmin(int groupId,[FromBody] string[] selUserName, [FromHeader] string Authorization)
		{
			string username = GetUsernameFromToken(Authorization);
			if (selUserName != null && username != null)
			{
				_groupService.makeAdmin(groupId, selUserName[0],username);
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("RemoveUser/{groupId}")]
		public IActionResult RemoveUser(int groupId, [FromBody] string[] selUserName, [FromHeader] string Authorization)
		{
			string username = GetUsernameFromToken(Authorization);
			if (selUserName != null && username != null)
			{
				_groupService.removeUser(groupId, selUserName[0], username);
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("SendFileMessage")]
		public IActionResult SaveFileMessage([FromForm] GMessageInModel msg)
		{
			string filetype = msg.File.ContentType.Split('/')[0];
			if (filetype == "audio" || filetype == "image" || filetype == "video")
			{
				_groupService.SendFileMessage(msg);
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("GetAllMessage")]
		public IActionResult GetMessage([FromBody] int groupId)
		{
			IEnumerable<GMessageSendModel> msgList = _groupService.GetAllMessage(groupId);
			if (msgList != null)
			{
				return Ok(msgList);
			}
			return Ok();
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
