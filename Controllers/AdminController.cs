using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Employee" )]
    public class AdminController : ControllerBase
    {

        private readonly IProfileService profileService;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IOnlineUserService onlineUserService;

        public AdminController(IProfileService profileService,
            IHubContext<ChatHub> hubContext,
            IOnlineUserService onlineUserService
            )
        {
            this.profileService = profileService;
            this.hubContext = hubContext;
            this.onlineUserService = onlineUserService;
        }

        [HttpGet("FetchAllEmployees")]
        public IActionResult FetchAllEmployees()
        {
            var employees = this.profileService.FetchAllProfiles().Select( 
             profile => new 
            {
                 FirstName = profile.FirstName,
                 LastName = profile.LastName,
                 UserName = profile.UserName,
                 Email = profile.Email,
                 ImageUrl = profile.ImageUrl,
                 Designation = profile.Designation
            });
            return Ok ( employees );
        }


        
        [HttpPatch("DeleteEmployee")]
        [Authorize(Policy = "Admin")]
        public IActionResult DeleteEmployees([FromBody] TempModel temp)
        {
            var employee = this.profileService.FetchProfileFromUserName(temp.UserName);
            if(employee != null )
            {
                OnlineUserEntity onlineUpdatedUser = this.onlineUserService.FetchOnlineUser(temp.UserName);
                if (onlineUpdatedUser != null)
                {
                    this.hubContext.Clients.Client(onlineUpdatedUser.ConnectionId).SendAsync("ProfileDeleted");
                }

                this.profileService.DeleteProfile(employee);
                return Ok(employee);
            }
            return BadRequest("Something went wrong");
         
        }

        [HttpPatch("UpdateEmployeeData")]
        [Authorize(Policy ="Admin")]
        public IActionResult UpdateEmployeeData([FromBody] UserModel employee, [FromQuery] string EmployeeOldUsername)
        {
            var profileFromDb = this.profileService.FetchProfileFromUserName(EmployeeOldUsername);
            if(profileFromDb != null )
            {
                profileFromDb.FirstName = employee.FirstName;
                profileFromDb.LastName = employee.LastName;
                profileFromDb.Email = employee.Email;
                profileFromDb.UserName = employee.UserName;
                profileFromDb.DesignationID = employee.Designation.Id;

                this.profileService.UpdateEmployeeProfile(profileFromDb);

                OnlineUserEntity onlineUpdatedUser = this.onlineUserService.FetchOnlineUser(EmployeeOldUsername);
                if(onlineUpdatedUser != null)
                {
                    this.hubContext.Clients.Client(onlineUpdatedUser.ConnectionId).SendAsync("ProfileUpdated");
                }
                return Ok();
            }
            return NotFound("No user found");
        }
    }
}
