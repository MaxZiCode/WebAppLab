using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Models;
using WebAppLab.Services;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ILogger<AuthController> logger;
		private readonly IAuthManager authManager;

		public AuthController(ILogger<AuthController> logger, IAuthManager authManager)
		{
			this.logger = logger;
			this.authManager = authManager;
		}

		[HttpPost("SignUp")]
		public async Task<IActionResult> SignUp([FromBody] AuthData authData)
		{
			var result = await authManager.SignUpAsync(HttpContext, authData.Email, authData.Password, Roles.User);
			if (!result.Succeeded)
				return BadRequest(new { errorMessage = result.FailMessage });

			logger.LogInformation("New user {user} just signed up", authData.Email);
			return StatusCode(201);
		}

		[HttpPost("SignIn")]
		public async Task<IActionResult> SignIn([FromBody] AuthData authData)
		{
			var result = await authManager.SignInAsync(authData.Email, authData.Password);

			if (!result.Succeeded)
			{
				logger.LogWarning(result.FailMessage);
				return Unauthorized();
			}
			return Ok();
		}

		[Authorize]
		[HttpPost("SignOut")]
		public async Task<IActionResult> SignOutPost()
		{
			var result = await authManager.SignOutAsync();
			if (!result.Succeeded)
				return BadRequest(new { errorMessage = result.FailMessage });

			return Ok();
		}

		[HttpGet("ConfirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			var result = await authManager.ConfirmEmailAsync(userId, code);
			if (!result.Succeeded)
			{
				logger.LogError(result.FailMessage);
				return BadRequest(new { errorMessage = result.FailMessage });
			}
			return Ok();
		}
	}
}
