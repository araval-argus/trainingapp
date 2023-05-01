using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Infrastructure.ServiceImplementation;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService) {
            _groupService= groupService;
        }

        [HttpPost("addgroup")]
        public IActionResult AddGroup([FromForm] GroupModel groupModel, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var savedGroup = _groupService.AddGroup(groupModel, claim.Value);
                if (savedGroup)
                {
                    return Ok(new { savedGroup });
                }
            }
            return response;
        }

        [HttpGet("getAll")]
        public IActionResult getAll([FromHeader]string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var groups = _groupService.getAll(claim.Value);
                if (groups != null)
                {
                    return Ok(new { groups });
                }
            }
            return response;
        }

        [HttpGet("getMembers")]
        public IActionResult getMembers([FromQuery]string name, [FromHeader]string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var members = _groupService.getMembers(claim.Value, name);
                response = Ok(members);
            }
            return response;
        }

        [HttpPost("addMembers")]
        public IActionResult addMember([FromBody]List<string> userNames, [FromHeader] string authorization, [FromQuery]string groupName)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var membersAdded = _groupService.addMembers(userNames, groupName, claim.Value);
                response = Ok(new { membersAdded });
            }
            return response;
        }


        [HttpPost("addMessage")]
        public IActionResult addMessage([FromBody] GroupReceiveChatModel message, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                
                var chatAdded = _groupService.addMessage(message, claim.Value);
                response = Ok(new { chatAdded });
            }
            return response;
        }

        [HttpGet("getAllChat")]
        public IActionResult getAllChat([FromQuery] string groupName, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {

                var chats = _groupService.getAllChat(groupName);
                response = Ok(new { chats });
            }
            return response;
        }

        [HttpPost("addFile")]
        public IActionResult addFile([FromForm] GroupChatFileModel fileMsg, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var chat = _groupService.addFile(fileMsg, claim.Value);
                response = Ok(new { chat });
            }
            return response;
        }

        [HttpPost("updateGroup")]
        public IActionResult updateGroup([FromForm] GroupModel group, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var updatedGroup = _groupService.UpdateGroup(group, claim.Value);
                if(updatedGroup)
                {
                    response = Ok(new { updatedGroup });
                }
            }
            return response;
        }

        [HttpPost("removeMember")]
        public IActionResult removeMember([FromBody] List<string> removeList, [FromHeader] string authorization, [FromQuery] string groupName)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var removeMember = _groupService.removeMember(removeList, groupName, claim.Value);
                if (removeMember)
                {
                    response = Ok(new { removeMember });
                }
            }
            return response;
        }


        [HttpGet("leaveGroup")]
        public IActionResult leaveGroup([FromQuery]string groupName, [FromHeader]string authorization) {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var leaveGroup = _groupService.leaveGroup(claim.Value, groupName);
                if (leaveGroup)
                {
                    response = Ok( leaveGroup );
                }
            }
            return response;

        }


        [HttpGet("getData")]
        public IActionResult getData([FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "You Are Not Authorized To Get Other's Data" });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var data = _groupService.getData(claim.Value);
                if (data != null)
                {
                    response = Ok(data);
                }
            }
            return response;

        }
    }
}
