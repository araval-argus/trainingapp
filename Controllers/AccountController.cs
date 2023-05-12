using System.IdentityModel.Tokens.Jwt;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ChatApp.Models.User;

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

		[Authorize]
		[HttpGet("{userName}")]
        public IActionResult GetUser(string userName)
        {
            Profile user = _profileService.GetUser(userName);
            if (user != null && user.IsDeleted == 0)
            {
                SelectedUserModel model = new SelectedUserModel();
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

		[Authorize]
		[HttpGet("GetStatusList")]
        public IActionResult StatusList()
        {
            return Ok(_profileService.GetAllStatus());
        }

		[Authorize]
		[HttpGet("Status/{userName}")]
        public IActionResult UserStatus(string userName)
        {
            var h = _profileService.getUserStatus(userName);
			return Ok(h);
        }

		[Authorize]
		[HttpPost("ChangeStatus/{userName}")]

		[Authorize]
		public IActionResult ChangeStatus(string userName , [FromBody] int statusCode )
        {
            _profileService.ChangeStatus(userName, statusCode);
            return Ok();
        }

		[Authorize]
		[HttpGet("GetUsers")]
        public IActionResult GetUsers([FromHeader]string Authorization)
        {
			string userName = GetUserNameFromToken(Authorization);
            return Ok(_profileService.getAllUsers(userName));
		}

        [Authorize]
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordModel password, [FromHeader] string Authorization)
        {
			IActionResult response = Unauthorized(new { Message = "Invalid Credentials." });
			var username = GetUserNameFromToken(Authorization);
			if (username != null)
			{
				bool isCorrect = _profileService.ChangePassword(username, password);
                if (isCorrect)
                {
                    response = Ok();
                }
                else
                {
                    return BadRequest(new { Message = "Invalid Credentials." });
                }
			}
			return response;
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