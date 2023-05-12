using ChatApp.Business.ServiceInterfaces;
using ChatApp.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy ="AdminPolicy")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminService _adminService;

		public AdminController(IAdminService adminService)
		{
			_adminService = adminService;
		}

		[HttpGet("GetDesignation")]
		public IActionResult GetAllDesignation([FromHeader] string Authorization)
		{
			string username = GetUserNameFromToken(Authorization);
			return Ok(_adminService.getAllDesignation(username));
		}

		[HttpDelete("DeleteUser/{userName}")]
		public IActionResult Delete(string userName, [FromHeader] string Authorization)
		{
			var loggedInUsername = GetUserNameFromToken(Authorization);
			bool deleted = _adminService.DeleteUser(userName, loggedInUsername);
			if (deleted == true)
			{
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("UpdateProfile")]
		public IActionResult UpdateProfile([FromForm] UpdateModel updateModel, [FromHeader] string Authorization)
		{
			string userName = GetUserNameFromToken(Authorization);
			IActionResult response = Unauthorized(new { Message = "Invalid Credentials." });
			if (userName != null)
			{
				_adminService.UpdateUser(updateModel,userName);
				response = Ok();
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
