using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net;
using ChatApp.Infrastructure.ServiceImplementation;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public AccountController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            IActionResult response = Unauthorized(new { Message = "Invalid Credentials." });
            var tokenString = _profileService.CheckPassword(loginModel);
            if (tokenString != null)
            {
                _profileService.ChangeStatus(loginModel.Username, 1);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterModel registerModel)
        {
            var tokenString = _profileService.RegisterUser(registerModel);
            if (tokenString != null)
            {
                return Ok(new { token = tokenString });
            }
            return BadRequest(new { Message = "User Already Exists. Please use different email and UserName." });
        }

        [Authorize]
        [HttpPost("update-profile")]
        public IActionResult UpdateProfile([FromForm] UpdateModel updateModel, [FromHeader] string Authorization)
        {
            string userName = GetUserNameFromToken(Authorization);
            var profile = _profileService.GetUser(userName);
			if (updateModel.UserName == userName || profile.Designation>7)
            {    //Method to update User Profile
                string tokenString = _profileService.UpdateUser(updateModel, userName);
                if (updateModel.UserName == userName)
                {
                    return Ok(new { token = tokenString });
                }
            }
            return BadRequest(new { Message = " Cannot Update User " });
        }

        [HttpGet("{userName}")]
        public IActionResult GetUser(string userName)
        {
            Profile user = _profileService.GetUser(userName);
            if (user != null && user.IsDeleted == 0)
            {
                ColleagueModel model = new ColleagueModel();
                model.UserName = user.UserName;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Email = user.Email;
                model.ImagePath = user.ImagePath;
                model.Designation = _profileService.GetDesignationFromId(user.Designation);
                model.Status = _profileService.getUserStatus(user.UserName).Status;
                return Ok(model);
            }
            return BadRequest();
        }

        [HttpGet("GetStatusList")]
        public IActionResult StatusList()
        {
            return Ok(_profileService.GetAllStatus());
        }

        [HttpGet("Status/{userName}")]
        public IActionResult UserStatus(string userName)
        {
            var h = _profileService.getUserStatus(userName);
			return Ok(h);
        }

        [HttpPost("ChangeStatus/{userName}")]
        public IActionResult ChangeStatus(string userName , [FromBody] int statusCode )
        {
            _profileService.ChangeStatus(userName, statusCode);
            return Ok();
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers([FromHeader]string Authorization)
        {
			string userName = GetUserNameFromToken(Authorization);
            return Ok(_profileService.getAllUsers(userName));
		}

        private string GetUserNameFromToken(string Authorization)
        {
			var handler = new JwtSecurityTokenHandler();
			string auth = Authorization.Split(' ')[1];
			var decodedToken = handler.ReadJwtToken(auth);

			return decodedToken.Claims.First(claim => claim.Type == "sub").Value;
		}
    }
}