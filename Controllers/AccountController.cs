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
        private IChatService chatService;

        public AccountController(IConfiguration config,
            IProfileService profileService,
            IWebHostEnvironment webHostEnvironment,
            IChatService chatService)
        {
            _config = config;
            _profileService = profileService;
            _validations = new Validations();
            _webHostEnvironment = webHostEnvironment;
            this.chatService = chatService;
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
                return Ok(new { token = tokenString, user });
            }
            return BadRequest(new { Message = "User Already Exists. Please use different email and UserName." });
        }

        [HttpPost("Update")]
        [Authorize]
        public IActionResult UpdateUserDetails([FromForm] UpdateModel updateModel, [FromHeader] string Authorization)
        {

            var usernameFromToken = CustomAuthorization.GetUsernameFromToken(Authorization);

            var user = _profileService.FetchProfileFromUserName(usernameFromToken);
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

        [HttpGet("FetchAllUsers")]
        [Authorize]
        public IActionResult FetchAllUsers([FromQuery] string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return BadRequest("enter username");
            }
            var users = this._profileService.FetchAllUsers(UserName);
            return Ok(users);
        }

        [HttpGet("FetchUser")]
        [Authorize]
        public IActionResult FetchUser([FromQuery] string UserName, [FromHeader] string Authorization)
        {
            if(string.IsNullOrEmpty(UserName))
            {
                return BadRequest("Invalid Username");
            }
            var user = this._profileService.FetchProfileFromUserName(UserName);
            var userModel = EntityToModel.ConvertToUserModel(user);

            string senderUserName = CustomAuthorization.GetUsernameFromToken(Authorization);
            var sender = this._profileService.FetchProfileFromUserName(senderUserName);

            var messages = this.chatService.FetchMessages(sender.Id, user.Id);

            if(messages.Count() > 0)
            {
                var lastMessage = messages.ToList()[messages.Count() - 1];
                userModel.LastMessage = lastMessage.Message;
                userModel.LastMessageTimeStamp = lastMessage.CreatedAt;
            }
           
            return Ok(userModel);
        }

        [HttpPatch("ChangeStatus")]
        [Authorize]
        public IActionResult ChangeStatus(int statusId, [FromHeader] string Authorization)
        {

            string username = CustomAuthorization.GetUsernameFromToken(Authorization);
            var user = this._profileService.FetchProfileFromUserName(username);
            if(user == null)
            {
                return BadRequest("user not found");
            }
            this._profileService.ChangeStatus(user, statusId);
            return Ok();
        }

        [HttpGet("FetchAllStatus")]
        [Authorize]
        public IActionResult FetchAllStatus()
        {
            var allStatus = this._profileService.FetchAllStatus();
            return Ok(allStatus);
        }

        [HttpGet("FetchStatus")]
        [Authorize]
        public IActionResult FetchStatus([FromHeader] string Authorization)
        {
            var userName = CustomAuthorization.GetUsernameFromToken(Authorization);
            var user = this._profileService.FetchProfileFromUserName(userName);
            if(user == null)
            {
                return Unauthorized("Unknown User");
            }
            var status = this._profileService.FetchStatus(user.Id);
            return Ok(status);
        }

        #region HelperMethods

        string GenerateJSONWebToken(Profile profileInfo)
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

        //stores the image and returns the name of that file
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