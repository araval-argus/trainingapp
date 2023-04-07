using ChatApp.Business.ServiceInterfaces;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
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
        private IConfiguration _config;
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
                if (savedChat)
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
    }
}
