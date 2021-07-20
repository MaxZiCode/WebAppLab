using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		private readonly ILogger<HomeController> logger;

		public HomeController(ILogger<HomeController> logger)
		{
			this.logger = logger;
		}

		[Authorize(Roles = Constants.Roles.Admin)]
		[HttpGet]
		public IActionResult GetInfo()
		{
			logger.LogInformation("GetInfo request executing");
			return Ok("Hello World");
		}
	}
}
