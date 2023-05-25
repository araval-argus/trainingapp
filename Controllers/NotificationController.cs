using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IProfileService profileService;
        private readonly INotificationService notificationService;

        public NotificationController(IProfileService profileService,
            INotificationService notificationService)
        {
            this.profileService = profileService;
            this.notificationService = notificationService;
        }

        [HttpGet("FetchNotifications")]
        public IActionResult FetchNotifications([FromHeader] string Authorization)
        {
            string userName = CustomAuthorization.GetUsernameFromToken(Authorization);
            var sender = this.profileService.FetchProfileFromUserName(userName);

            if(sender == null) 
            {
                return BadRequest("user not found");
            }

            var notifications = this.notificationService.GetAllNotifications(sender.Id);

            return Ok(notifications);
        }


        [HttpDelete("ClearAllNotifications")]
        public IActionResult ClearAllNotifications([FromHeader] string Authorization)
        {
            string userName = CustomAuthorization.GetUsernameFromToken(Authorization);
            var sender = this.profileService.FetchProfileFromUserName(userName);

            var notifications = this.notificationService.GetAllNotifications(sender.Id);
            this.notificationService.DeleteNotifications(notifications);

            return Ok(notifications);
        }
    }
}
