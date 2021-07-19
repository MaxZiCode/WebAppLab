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
		private readonly IMapper mapper;
		private readonly ApplicationDbContext context;

		public UserController(IMapper mapper, ApplicationDbContext context)
		{
			this.mapper = mapper;
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<User>> Get()
		{
			var id = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			if (id == null)
				return null;

			var userFromDb = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
			if (userFromDb == null)
				return null;

			var user = mapper.Map<UserDbo, User>(userFromDb);

			return user;
		}

		[HttpPut]
		public async Task<ActionResult<User>> Put([FromBody] User user)
		{
			var id = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var userDbo = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

			mapper.Map(user, userDbo);
			try
			{
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				string message = "Unable to save changes. " +
					"Try again, and if the problem persists, " +
					"see your system administrator.";
				return BadRequest(new { message });
			}
			var changedUserDbo = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
			var changedUser = mapper.Map<User>(userDbo);
			return changedUser;
		}
	}
}
