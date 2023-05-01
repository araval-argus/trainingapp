    using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class AccountController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IProfileService _profileService;
        public AccountController(IConfiguration config, IProfileService profileService)
        {
            _config = config;
            _profileService = profileService;
        }

        [HttpPost("Login")]
        [Authorize]

        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            IActionResult response = Unauthorized(new { Message = "Invalid Credentials."});
            var user = _profileService.CheckPassword(loginModel);

            if (user != null)
            {
                _profileService.setStatus(loginModel.Username, 1);
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpPost("Register")]
        [Authorize]

        public IActionResult Register([FromBody] RegisterModel registerModel)
        {
            var user = _profileService.RegisterUser(registerModel);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                return Ok(new { token = tokenString, user = user });
            }
            return BadRequest(new { Message = "User Already Exists. Please use different email and UserName." });
        }

        [HttpPut("update/{username}")]
        [Authorize]

        public IActionResult update([FromForm] UpdateModel updateModel,string username)
        {
            var user = _profileService.UpdateUser(updateModel, username);
            // service will return null if user not found based on username
            if(user == null)
            {
                return BadRequest(new { Message = "User Not Found" });
            }
            // service will return new profile
            if(user.UserName == null)
            {
                return BadRequest(new { Message = "Email is already used in different account" });
            }
            var tokenString = GenerateJSONWebToken(user);
            return Ok(new { token = tokenString });
        }

        [HttpGet("getprofiles")]
        [Authorize]
        public IActionResult getprofiles([FromQuery]string s, [FromHeader]string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                var profiles = _profileService.GetProfileDTOs(s, claim.Value);
                return Ok(profiles);

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
            if(claim != null)
            {
                List<profileDTO> profiles = _profileService.getAll();
                response = Ok(profiles);
            }
            return response;
        }

        [HttpGet("getImage")]
        [Authorize]

        public IActionResult GetImage(string userName) { 
            Profile profile = _profileService.getUserFromUserName(userName);
            string imgPath = _profileService.GetImage(profile.Id);
            if(imgPath == null)
            {
                return BadRequest(new { Message = "Email is already used in different account" });
            }
            return Ok(new { imgPath = imgPath });
        }

        [HttpGet("setStatus")]
        [Authorize]
        public IActionResult setStatus([FromHeader] string authorization, [FromQuery]int id)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                bool statusUpdated = _profileService.setStatus(claim.Value, id);
                response = Ok(statusUpdated);
            }
            return response;
        }


        [HttpPost("changePassword")]
        [Authorize]
        public IActionResult changePassword([FromForm] ChangePasswordModel newPasswordModel, [FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Something Went Wrong." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if (claim != null)
            {
                bool passwordChanged = _profileService.changePassword(newPasswordModel, claim.Value);
                response = Ok(passwordChanged);
            }
            return response;
        }

        private string GenerateJSONWebToken(Profile profileInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, profileInfo.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, profileInfo.Email),
                    new Claim(ClaimsConstant.FirstNameClaim, profileInfo.FirstName),
                    new Claim(ClaimsConstant.LastNameClaim, profileInfo.LastName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}