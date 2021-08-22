
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

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
		public async Task<ActionResult<UserViewModel>> GetUser() => Ok(await userChanger.GetUserAsync(HttpContext.User));

		[HttpPut]
		public async Task<ActionResult<UserViewModel>> ChangeUser([FromBody] UserViewModel user)
		{
			var result = await userChanger.ChangeAsync(HttpContext.User, user);
			if (!result.Succeeded)
				return BadRequest(new { errorMessage = result.FailMessage });

			return Ok(result.Data);
		}

		[HttpPatch("password")]
		public async Task<IActionResult> ChangePassword([FromBody] JsonPatchDocument<UserPassword> jsonPatch)
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
