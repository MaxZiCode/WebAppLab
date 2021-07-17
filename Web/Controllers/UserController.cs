using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Models;

namespace WebAppLab.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;

		public UserController(ApplicationDbContext context, IMapper mapper)
		{
			this.context = context;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<User>> Get()
		{
			var id = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var userFromDb = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
			var user = mapper.Map<UserDbo, User>(userFromDb);
			return user;
		}
	}
}
