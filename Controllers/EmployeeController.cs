using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Infrastructure.ServiceImplementation;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController( IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(Policy = "EmployeePolicy")]
        //[Authorize(Roles = "Admin")]
        [HttpGet("getAll")]
        public IActionResult getAll()
        {
            IActionResult response = Unauthorized(new { Message = "Unathorized." });
            List<profileDTO> profileDTOs = _employeeService.getAllUser();
            response = Ok(profileDTOs);
            return response;
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("updateRole")]
        public IActionResult updateRole([FromQuery]string userName, [FromQuery]string profileType, [FromHeader]string authorization)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            IActionResult response = Unauthorized(new { Message = "Unathorized." });
            bool updatedRole = _employeeService.updateRole(userName, profileType, claim.Value);
            response = Ok(updatedRole);
            return response;
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("deleteUser")]
        public IActionResult deleteUser([FromQuery] string userName, [FromHeader]string authorization)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            IActionResult response = Unauthorized(new { Message = "Unathorized." });
            bool deletedUser = _employeeService.deleteUser(userName, claim.Value);
            response = Ok(deletedUser);
            return response;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("addUser")]
        public IActionResult addUser([FromBody] RegisterModel regModel, [FromHeader]string authorization)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            IActionResult response = Unauthorized(new { Message = "User Already Exist." });
            if(claim == null)
            {
                response = Unauthorized(new { Message = "Unauthorized" });
                return response;
            }
            bool newUserAdded = _employeeService.RegisterUser(regModel, claim.Value);
            if (newUserAdded)
            {
                response = Ok(newUserAdded);
            }
            return response;
        }



    }
}
