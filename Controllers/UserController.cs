using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Assets;
using ChatApp.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace ChatApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IConfiguration config;
        private readonly IUserService userService;
        private readonly IAvatarService assetService;

        public UserController(IConfiguration config, IUserService userService, IAvatarService assetService)
        {
            this.config = config;
            this.userService = userService;
            this.assetService = assetService;
        }


        [Route("all")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            IEnumerable<UserModel> users =  userService.GetUsers();
            return Ok(users);
        }

        [Route("{username}")]
        [HttpGet]
        public IActionResult GetUser(string username)
        {
            UserModel user = userService.GetUserByUsername(username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Route("{username}")]
        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody] UserUpdateModel userDetails, string username)
        {
            UserModel user = await userService.UpdateUser(userDetails, username);

            // update error
            if (user == null)
            {
                return BadRequest();
            }

            return Ok(user);
        }

        [Route("{username}/profileUpload")]
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadProfile(string username, IFormFile profileImage)
        {
            // check if the user present in the database
            UserModel user = userService.GetUserByUsername(username);

            if (user == null)
            {
                return BadRequest("No user found!");
            }

            if (profileImage.Length == 0)
            {
                return BadRequest("No image found");
            }

            if (profileImage.Length > int.MaxValue)
            {
                return BadRequest("Too large file");
            }

            AvatarModel asset = assetService.SaveProfileImage(user, profileImage);

            return Ok( new { asset.FilePath });
        }

        
        [Route("search/{user}")]
        [HttpGet]
        public IActionResult SearchUser(string user)
        {
            var searchResult = userService.SearchUser(user);

            return Ok(searchResult);
        }


       
    }
}
