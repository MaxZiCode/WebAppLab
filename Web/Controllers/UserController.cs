using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Models;

namespace WebAppLab.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserSearcher searcher;

		public UserController(IUserSearcher searcher)
		{
			this.searcher = searcher;
		}

		[HttpGet]
		public async Task<ActionResult<User>> Get()
		{
			var user = await searcher.FindAsync(HttpContext);
			if (user == null)
				return NotFound();

			return user;
		}
	}
}
