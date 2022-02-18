using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IUserService _userService;

        public UserController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }


        [Route("all")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            IEnumerable<UserModel> users =  _userService.GetUsers();
            return Ok(users);
        }

        [Route("{username}")]
        [HttpGet]
        public IActionResult GetUser(string username)
        {
            UserModel user = _userService.GetUserByUsername(username);

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
            UserModel user = await _userService.UpdateUser(userDetails, username);

            // update error
            if (user == null)
            {
                return BadRequest();
            }

            return Ok(user);
        }

    }
}
