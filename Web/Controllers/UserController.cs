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
using WebAppLab.Services;

namespace WebAppLab.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserChanger userChanger;

		public UserController(IUserChanger userChanger)
		{
			this.userChanger = userChanger;
		}

		[HttpGet]
		public async Task<ActionResult<User>> Get() => await userChanger.GetUserAsync(HttpContext.User);

		[HttpPut]
		public async Task<ActionResult<User>> Put([FromBody] User user)
		{
			var result = await userChanger.ChangeAsync(HttpContext.User, user);
			if (!result.Succeeded)
				return BadRequest(new { errorMessage = result.Error });

			return result.User;
		}

		[HttpPatch("password")]
		public async Task<IActionResult> PatchPassword([FromBody] JsonPatchDocument<UserPassword> jsonPatch)
		{
			UserPassword userPassword = new();
			jsonPatch.ApplyTo(userPassword);

			var result = await userChanger.ChangePasswordAsync(HttpContext.User, userPassword.Password);
			if (!result.Succeeded)
				return BadRequest(new { errorMessage = result.FailMessage });

			return Ok();
		}
	}
}
