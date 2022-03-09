using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
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
        public IActionResult GetTokenInfo(string userNameTo)
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

    }
}
