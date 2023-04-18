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

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            IActionResult response = Unauthorized(new { Message = "Invalid Credentials." });
            var user = _profileService.CheckPassword(loginModel);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpPost("Register")]
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

        [Authorize]
        [HttpPost("update-profile")]
        public IActionResult UpdateProfile([FromForm] UpdateModel updateModel, [FromHeader] string Authorization)
        {
            var handler = new JwtSecurityTokenHandler();
            string auth = Authorization.Split(' ')[1];
            var decodedToken = handler.ReadJwtToken(auth);

            string userName = decodedToken.Claims.First(claim => claim.Type == "sub").Value;

            if (updateModel.UserName == userName)
            {    //Method to update User Profile
                var user = _profileService.UpdateUser(updateModel, userName);
                {
                    var tokenString = GenerateJSONWebToken(user);
                    return Ok(new { token = tokenString, user = user });
                }
            }
            return BadRequest(new { Message = " Cannot Update User " });
        }

        [HttpGet("{userName}")]
        public IActionResult GetUser(string userName)
        {
            Profile user = _profileService.GetUser(profile => profile.UserName == userName);
            ColleagueModel model = new ColleagueModel();
            model.UserName = user.UserName;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            model.ImagePath = user.ImagePath;
            model.Designation = user.Designation;
            return Ok(model);
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
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimsConstant.ImagePathClaim, profileInfo.ImagePath),
                    new Claim(ClaimsConstant.DesignationClaim,profileInfo.Designation)
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