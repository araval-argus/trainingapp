using ChatApp.Business.ServiceInterfaces;
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
        [Authorize]
        public IActionResult AddGroup([FromForm] GroupModel groupModel, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var savedGroup = _groupService.AddGroup(groupModel, claim.Value);
                if (savedGroup != null)
                {
                    return Ok(new { savedGroup });
                }
            }
            return response;
        }

        [HttpGet("getAll")]
        [Authorize]
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
        [Authorize]
        public IActionResult getMembers([FromQuery]string name, [FromHeader]string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var members = _groupService.getMembers(claim.Value, name);
                response = Ok(new { members });
            }
            return response;
        }

        [HttpPost("addMembers")]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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


    }
}
