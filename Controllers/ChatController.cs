using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ChatController : ControllerBase
    {

        private readonly IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet("fetchFriends")]
        [Authorize]
        public IActionResult FetchFriends(string searchTerm)
        {
            //IEnumerable<FriendProfileModel> en = this.chatService.FetchFriendsProfiles(searchTerm);
            //Console.WriteLine("inside FetchFriends of chat controller");
            //foreach(var profile in en)
            //{

            //    Console.WriteLine(profile.FirstName);
            //}

            Console.WriteLine(searchTerm);
            IEnumerable<FriendProfileModel> en = this.chatService.FetchFriendsProfiles(searchTerm);
            foreach(FriendProfileModel profile in en)
            {
                Console.WriteLine(profile.FirstName);
            }
            return Ok(new { message = en });
        }
    }
}
