using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController: ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IChatService chatService;
        private readonly IUserService userService;

        public ChatController( IConfiguration config, IChatService chatService, IUserService userService)
        {
            this.config = config;
            this.chatService = chatService;
            this.userService = userService;
        }


        [Route("direct/{userNameTo}/")]
        [HttpGet]
        public IActionResult GetChatHistoryWithUser(string userNameTo)
        {
            // returns chat with the user provided in url

            string userNameFrom = JwtHelper.GetUsernameFromRequest(Request);

            if (userNameFrom == "")
            {
                return BadRequest();
            }

            var userFrom = userService.GetUserByUsername(userNameFrom);

            if (userFrom == null)
            {
                return BadRequest();
            }

            var userTo = userService.GetUserByUsername(userNameTo);

            if (userTo == null)
            {
                return BadRequest();
            }

            var chat = chatService.chatLists(userFrom.Id, userTo.Id);

            return Ok(chat);
        }

        [Route("direct/{userNameTo}/")]
        [HttpPost]
        public IActionResult SendMessageTo([FromRoute] string userNameTo, [FromBody] ChatSendMessage message)
        {
            string userNameFrom = JwtHelper.GetUsernameFromRequest(Request);

            if (userNameFrom == "")
            {
                return BadRequest();
            }

            if (CheckValidUser(userNameFrom) == -1 || CheckValidUser(userNameTo) == -1 )
            {
                return BadRequest();
            }

            if (message.Sender != userNameFrom || message.Receiver != userNameTo)
            {
                return BadRequest();
            }

            if (message.Type == "text")
            {
                var sendMessage = chatService.sendTextMessage(userNameFrom, userNameTo, message.Content);
                return Ok(new {status= "Success", message=sendMessage });
            }

            return BadRequest("This request type isn't implemented by the server.");
        }

        [Route("getRecentChatUsers/{userName}")]
        [HttpGet]
        public IActionResult GetRecentChatUsers(string userName)
        {
            string userNameFromJWT = JwtHelper.GetUsernameFromRequest(Request);

            if (userNameFromJWT == "")
            {
                return BadRequest("JWT Token tampered!");
            }

            var userId = CheckValidUser(userName);

            if (userId == -1)
            {
                return BadRequest("Not a valid user");
            }

            if (userName != userNameFromJWT)
            {
                return BadRequest("User from Token and URL mismatched!");
            }

            var users = chatService.recentChatUsers(userId);

            return Ok(users);

        }

        private int CheckValidUser (string userName)
        {
            var userInfo = userService.GetUserByUsername(userName);

            if (userInfo != null)
            {
                return userInfo.Id;
            }

            return -1;
        }

    }
}
