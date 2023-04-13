using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DesignationEntity = ChatApp.Context.EntityClasses.DesignationEntity;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    
    public class AccountController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IProfileService _profileService;
        private Validations _validations;
        private IWebHostEnvironment _webHostEnvironment;

        public AccountController(IConfiguration config, IProfileService profileService, IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            _profileService = profileService;
            _validations = new Validations();
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            IActionResult response = Unauthorized(new { Message = "Invalid Credentials."});
            
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
            if (!_validations.ValidateRegistrationField(registerModel))
            {
                return BadRequest(new { Message = "fields cant be validated" });
            }
            var user = _profileService.RegisterUser(registerModel);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                return Ok(new { token = tokenString, user = user });
            }
            return BadRequest(new { Message = "User Already Exists. Please use different email and UserName." });
        }

        [HttpPost("Update")]
        [Authorize]
        public IActionResult UpdateUserDetails([FromForm] UpdateModel updateModel, [FromHeader] string Authorization)
        {

            var usernameFromToken = GetUsernameFromToken(Authorization);

            var user = _profileService.FetchProfile(usernameFromToken);
            if (user != null)
            {
                if (updateModel.ImageFile != null)
                {
                    DeleteOlderImage(user.ImageUrl);
                    user.ImageUrl = createUniqueImgFile(updateModel.ImageFile);
                    user = _profileService.UpdateProfile(updateModel, usernameFromToken, true);
                }
                else
                {
                    user = _profileService.UpdateProfile(updateModel, usernameFromToken);
                }
                var tokenString = GenerateJSONWebToken(user);
                return Ok(new { token = tokenString, Message = "Your details has been updated successfully" });
             }
                return BadRequest(new { Message = "User does not exists" });
        }

        [HttpGet("checkUsername")]
        public IActionResult CheckUsername([FromQuery] string username)
        {
            return Ok(new { usernameExists = _profileService.CheckUserNameExists(username) });
        }


        [HttpGet("fetchDesignations")]
        public IActionResult FetchDesignations()
        {
            IEnumerable<DesignationEntity> designations = this._profileService.FetchAllDesignations();
            return Ok(new { designations = designations });
        }

        [HttpPost("checkPassword")]
        [Authorize]
        public IActionResult CheckPassword([FromBody] LoginModel loginModel)
        {
            if (this._profileService.CheckPassword(loginModel) != null)
            {
                return Ok(new {passwordMatched = true});
            }

            return Ok(new { passwordMatched = false });
        }

        [HttpPost("changePassword")]
        [Authorize]
        public IActionResult ChangePassword(PasswordModel passwordModel)
        {
            if(this._profileService.ChangePassword(passwordModel))
                return Ok(new { message = "password changed"});

            return BadRequest(new { message = "password incorrect" });
        }

        #region HelperMethods

        private string GenerateJSONWebToken(Profile profileInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, profileInfo.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, profileInfo.Email),
                    new Claim(ClaimsConstant.FirstNameClaim, profileInfo.FirstName),
                    new Claim(ClaimsConstant.LastNameClaim, profileInfo.LastName),
                    new Claim(ClaimsConstant.ImageUrlClaim, profileInfo.ImageUrl),
                    new Claim(ClaimsConstant.DesignationClaim, profileInfo.Designation.Designation),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        string GetUsernameFromToken(string token)
        {
            string newToken = token.Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(newToken);
            return jsonToken.Claims.First(claim => claim.Type == "sub").Value;
        }

        string createUniqueImgFile(IFormFile imageFile)
        {

            string fileName = Guid.NewGuid().ToString();

            var location = Path.Combine(_webHostEnvironment.WebRootPath, @"Images/Users");

            var extension = Path.GetExtension(imageFile.FileName);

            using (FileStream fileStream = new FileStream(Path.Combine(location, fileName + extension), FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return fileName + extension;
        }

        void DeleteOlderImage(string path)
        {
            var location = Path.Combine(_webHostEnvironment.WebRootPath, @"Images/Users", path);
            if (System.IO.File.Exists(location))
            {
                System.IO.File.Delete(location);
            }
        }
        #endregion
    }
}