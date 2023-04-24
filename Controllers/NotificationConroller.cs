using ChatApp.Business.ServiceInterfaces;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class NotificationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        public NotificationController(IConfiguration config, INotificationService notificationService)
        {
            _config = config;
            _notificationService = notificationService;
        }


        [HttpGet("getAll")]
        [Authorize]
        public IActionResult AddChat([FromHeader] string authorization)
        {
            IActionResult response = Unauthorized(new { Message = "Unauthorized." });
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            if(claim != null)
            {
                var notifications = _notificationService.getAll(claim.Value);
                if (notifications != null)
                {
                    return Ok(new { notifications });
                }
            }
            return response;
        }

    }
}
