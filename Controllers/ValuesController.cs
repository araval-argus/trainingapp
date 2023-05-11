using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly IChatService chatService;
        private readonly IProfileService profileService;
        private readonly IGroupMessageService groupMessageService;

        public ValuesController(IChatService chatService,
            IProfileService profileService,
            IGroupMessageService groupMessageService)
        {
            this.chatService = chatService;
            this.profileService = profileService;
            this.groupMessageService = groupMessageService;
        }
        [HttpGet("FetchInsights")]
        public IActionResult FetchInsights([FromHeader] string Authorization)
        {
            string userName = CustomAuthorization.GetUsernameFromToken(Authorization);
            var user = this.profileService.FetchProfileFromUserName(userName);

            if(user == null)
            {
                return BadRequest("User does not exist");
            }

            var personalMessages = this.chatService.FetchAllMessages(user.Id);
            var groupMessages = this.groupMessageService.FetchAllGroupMessages(user.Id);

            var obj = Insights.PrepareInsights(personalMessages, groupMessages);

            return Ok(obj);
        }
    }
}
