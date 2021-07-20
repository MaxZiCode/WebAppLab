using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
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
		private readonly UserManager<UserDbo> userManager;

		public UserController(IMapper mapper, ApplicationDbContext context, UserManager<UserDbo> userManager)
		{
			this.mapper = mapper;
			this.context = context;
			this.userManager = userManager;
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

			var user = mapper.Map<User>(userFromDb);

			return user;
		}

		[HttpPut]
		public async Task<ActionResult<User>> Put([FromBody] User user)
		{
			bool isEmailCorrect = CheckUtility.CheckEmail(user.Email);
			if (!isEmailCorrect)
				return BadRequest();

			bool isPhoneNumberCorrect = CheckUtility.CheckPhoneNumber(user.PhoneNumber);
			if (!isPhoneNumberCorrect)
				return BadRequest();

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

		[HttpPatch("password")]
		public async Task<IActionResult> PatchPassword([FromBody] JsonPatchDocument<UserPassword> jsonPatch)
		{
			UserPassword userPassword = new();
			jsonPatch.ApplyTo(userPassword);

			bool isPasswordCorrect = CheckUtility.CheckPassword(userPassword.Password);
			if (!isPasswordCorrect)
				return ValidationProblem();

			var id = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var userDbo = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

			var hashedPassword = userManager.PasswordHasher.HashPassword(userDbo, userPassword.Password);

			userDbo.PasswordHash = hashedPassword;

			await context.SaveChangesAsync();
			return Ok();
		}
	}
}
