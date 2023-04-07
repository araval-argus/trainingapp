using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IDesignationService _designationService;

        public SampleController(IDesignationService designationService)
        {
            this._designationService = designationService;
        }

        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new List<string>() { "Test", "Api", "Run" };
        }

        [HttpPost("designation")]
        public IActionResult AddDesignation(string designationName)
        {
            this._designationService.AddDesignation(designationName);
            return Ok(new { message = "new designation added" });
        }
    }
}
