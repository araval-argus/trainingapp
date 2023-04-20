using ChatApp.Business.ServiceInterfaces;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Employee" )]
    public class AdminController : ControllerBase
    {

        private readonly IProfileService profileService;
        public AdminController(IProfileService profileService)
        {
            this.profileService = profileService;
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


        //Doubt :- how to fetch a single string from body without creating model
        [HttpPatch("DeleteEmployee")]
        [Authorize(Policy = "Admin")]
        public IActionResult DeleteEmployees([FromBody] TempModel temp)
        {
            var employee = this.profileService.FetchProfile(temp.UserName);
            if(employee != null )
            {
                this.profileService.DeleteProfile(employee);
                return Ok(employee);
            }
            return BadRequest("Something went wrong");
         
        }

        [HttpPatch("UpdateEmployeeData")]
        [Authorize(Policy ="Admin")]
        public IActionResult UpdateEmployeeData([FromBody] FriendProfileModel employee)
        {
            var profileFromDb = this.profileService.FetchProfile(employee.UserName);
            if(profileFromDb != null )
            {
                profileFromDb.FirstName = employee.FirstName;
                profileFromDb.LastName = employee.LastName;
                profileFromDb.Email = employee.Email;
                profileFromDb.UserName = employee.UserName;
                profileFromDb.DesignationID = employee.Designation.Id;

                this.profileService.UpdateEmployeeProfile(profileFromDb);
                return Ok();
            }
            return BadRequest("No user found");
        }
    }
}
