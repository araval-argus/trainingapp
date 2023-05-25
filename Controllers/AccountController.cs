using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private UserManager<Profile> userManager;

        public AccountController(IConfiguration config,
            IProfileService profileService,
            IWebHostEnvironment webHostEnvironment,
            IChatService chatService,
            UserManager<Profile> userManager)
        {
            _config = config;
            _profileService = profileService;
            _validations = new Validations();
            _webHostEnvironment = webHostEnvironment;
            this.chatService = chatService;
            this.userManager = userManager;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            IActionResult response = Unauthorized(new { Message = "Invalid Credentials."});

            var user = userManager.FindByNameAsync(loginModel.Username);

            if (user.Result == null)
            {
                user = userManager.FindByEmailAsync(loginModel.EmailAddress);
                if(user.Result == null)
                {
                    return BadRequest("User does not exist");
                }
            }

            if(userManager.CheckPasswordAsync(user.Result, loginModel.Password).Result)
            {
                var userToGenerateToken = this._profileService.FetchProfileFromUserName(user.Result.UserName);
                var tokenString = GenerateJSONWebToken(userToGenerateToken);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!_validations.ValidateRegistrationField(registerModel))
            {
                return BadRequest(new { Message = "fields cant be validated" });
            }
            var user = new Profile
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                //Password = regModel.Password,
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                CreatedAt = DateTime.UtcNow,
                ProfileType = ProfileType.User,
                ImageUrl = SetDefaultImage(),
                DesignationID = registerModel.DesignationID,                
                StatusID = 1,
                IsActive = true
            };

            var result = await this.userManager.CreateAsync(user, registerModel.Password);


            if (result.Succeeded)
            {
                //check whether the role inserted is correct or not
                result = await userManager.AddToRoleAsync(user, "User");

                if(result.Succeeded)
                {
                    user = this._profileService.FetchProfileFromUserName(registerModel.UserName);
                    var tokenString = GenerateJSONWebToken(user);
                    return Ok(new { token = tokenString });
                }

                
            }
            return BadRequest(new { Message = "Something Went Wrong" });
        }

        [HttpPost("Update")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetails([FromForm] UpdateModel updateModel, [FromHeader] string Authorization)
        {

            var usernameFromToken = CustomAuthorization.GetUsernameFromToken(Authorization);

            var user = _profileService.FetchProfileFromUserName(usernameFromToken);
            if (user != null)
            {
                if (updateModel.ImageFile != null)
                {
                    DeleteOlderImage(user.ImageUrl);
                    user.ImageUrl = createUniqueImgFile(updateModel.ImageFile);
                }

                user.FirstName = updateModel.FirstName;
                user.LastName = updateModel.LastName;
                user.Email = updateModel.Email;
                user.UserName = updateModel.UserName;

                var result = await userManager.UpdateAsync(user);
                if(result.Succeeded)
                {
                    var tokenString = GenerateJSONWebToken(user);
                    return Ok(new { token = tokenString, Message = "Your details has been updated successfully" });
                }

                return BadRequest("Something went wrong");

            }
            return BadRequest(new { Message = "User does not exists" });
        }

        [HttpGet("checkUsername")]
        public async Task<IActionResult> CheckUsername([FromQuery] string username)
        {
            var profile = await userManager.FindByNameAsync(username);
            return Ok(new { usernameExists = profile != null});
        }


        [HttpGet("fetchDesignations")]
        public IActionResult FetchDesignations()
        {
            IEnumerable<DesignationEntity> designations = this._profileService.FetchAllDesignations();
            return Ok(new { designations });
        }

        [HttpPost("checkPassword")]
        [Authorize]
        public async Task<IActionResult> CheckPassword([FromBody] LoginModel loginModel)
        {
            //loginModel is used to check the password as it contains all the necessary fields like username, email and password.
            var profile = await userManager.FindByNameAsync(loginModel.Username);
            if(profile == null)
            {
                profile = await userManager.FindByEmailAsync(loginModel.EmailAddress);
                if(profile == null)
                {
                    return BadRequest("Something went wrong");
                }
            }
            var result = await userManager.CheckPasswordAsync(profile,loginModel.Password);           

            return Ok(new { passwordMatched = result });
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(PasswordModel passwordModel)
        {
            var profile = _profileService.FetchProfileFromUserName(passwordModel.UserName);

            if (profile != null)
            {
                var result = await userManager.ChangePasswordAsync(profile, passwordModel.OldPassword, passwordModel.NewPassword);
                if(result.Succeeded)
                {
                    return Ok(new {message = "Password Changed" });
                }
                return BadRequest("Something went wrong");
            }
            return BadRequest("User not found");
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

        private string SetDefaultImage()
        {
            string fileName = Guid.NewGuid().ToString() + ".jpg";

            var DefaultFileLoc = Path.Combine(_webHostEnvironment.WebRootPath, @"Default/default_profile.jpg");
            var FileLoc = Path.Combine(DefaultFileLoc, @"../../Images/Users", fileName);

            System.IO.File.Copy(DefaultFileLoc, FileLoc);

            return fileName;
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