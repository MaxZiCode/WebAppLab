using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
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
			try
			{
				await authManager.SignUpAsync(HttpContext, authData.Email, authData.Password, Roles.User);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { errorMessage = ex.Message });
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex.Message);
				return BadRequest(new { errorMessage = ex.Message });
			}
			logger.LogInformation("New user {user} just signed up", authData.Email);
			return StatusCode(201);
		}

		[HttpPost("SignIn")]
		public async Task<IActionResult> SignIn([FromBody] AuthData authData)
		{
			try
			{
				await authManager.SignInAsync(authData.Email, authData.Password);
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex.Message);
				return Unauthorized();
			}
			return Ok();
		}

		[Authorize]
		[HttpPost("SignOut")]
		public async Task<IActionResult> SignOutPost()
		{
			await authManager.SignOutAsync();
			return Ok();
		}

		[HttpGet("ConfirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			try
			{
				await authManager.ConfirmEmail(userId, code);

			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return BadRequest(new { errorMessage = ex.Message });
			}
			HttpContext.Response.ContentType = "text/html; charset=utf-8";
			return Content("Email confirmed!");
		}
	}
}
