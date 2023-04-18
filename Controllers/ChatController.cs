using ChatApp.Business.ServiceInterfaces;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ChatController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IChatService _chatService;
        public ChatController(IConfiguration config, IChatService chatService)
        {
            _config = config;
            _chatService = chatService;
        }


        [HttpPost("addChat")]
        [Authorize]
        public IActionResult AddChat([FromBody] ChatModel chatModel, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if(claim != null)
            {
                var savedChat = _chatService.AddChat(chatModel, claim.Value);
                if (savedChat != null)
                {
                    return Ok(new { savedChat });
                }
                else
                {
                    return response;
                }
            }
            return response;
        }

        [HttpGet("getChat")]
        [Authorize]
        public IActionResult getChat([FromQuery]string s, [FromHeader]string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if(claim != null)
            {
                var chats = _chatService.GetAllChats(claim.Value, s);
                response = Ok(new { chats });
            }
            return response;
        }

        [HttpGet("recent")]
        [Authorize]
        public IActionResult recent([FromHeader]string authorization)
        {
            IActionResult response = Unauthorized(new { message = "Something Went Wrong" });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var chats = _chatService.recent(claim.Value);
                response = Ok(new { chats });
            }
            return response;
        }

        [HttpPost("addFile")]
        [Authorize]
        public IActionResult addFile([FromForm] ChatFileModel chatFile, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { message = "Something Went Wrong" });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if(claim!= null)
            {
                var chats = _chatService.addFile(claim.Value, chatFile);
                if(chats != null)
                {
                    response = Ok(new { chats });
                }
            }
            return response;
        }

        [HttpGet("markAsRead")]
        [Authorize]
        public IActionResult markAsRead([FromQuery] string s, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { message = "Something Went Wrong" });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var chats = _chatService.markAsRead(claim.Value, s);
                response = Ok(new { chats });
            }
            return response;
        }
    }
}
