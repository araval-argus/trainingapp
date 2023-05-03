using ChatApp.Business.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ChatApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DashBoardController : ControllerBase
	{
		private readonly IDashBoardService _dashboardService;
		public DashBoardController(IDashBoardService dashBoardService) 
		{
			_dashboardService = dashBoardService;
		}

		[HttpGet("Chart")]
		public IActionResult Chart()
		{
			int[] chat;
			int[] group;
			List<DateTime> dates = _dashboardService.chartDetails(out chat, out group);
			return Ok(new {chat,group,dates});
		}
	}
}
