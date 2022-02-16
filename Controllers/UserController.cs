using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
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
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IUserService _userService;

        public UserController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }


        //[Authorize]
        //[HttpGet]
        //public IActionResult GetUser()
        //{
        //    string[] s = { "hf", "dj", "djd" };

        //    return Ok(s);
        //}

        [Route("all")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            IEnumerable<UserModel> users = await _userService.GetUsers();
            return Ok(users);
        }

    }
}
