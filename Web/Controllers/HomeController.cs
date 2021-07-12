using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	public class HomeController : Controller
	{
		[HttpGet]
		public IActionResult GetInfo()
		{
			Log.Fatal("GetInfo request executes");
			return Json("Hello World");
		}
	}
}
