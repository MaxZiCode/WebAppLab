using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> logger;

		public HomeController(ILogger<HomeController> logger)
		{
			this.logger = logger;
		}

		[HttpGet]
		public IActionResult GetInfo()
		{
			logger.LogInformation("GetInfo request executing");
			return Ok("Hello World");
		}
	}
}
