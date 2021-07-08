using Microsoft.AspNetCore.Mvc;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	public class HomeController : Controller
	{
		[HttpGet]
		public IActionResult GetInfo()
		{
			return Json("Hello World");
		}
	}
}
