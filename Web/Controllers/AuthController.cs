using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Helpers;
using WebAppLab.Models;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ILogger<AuthController> logger;
		private readonly UserManager<UserDbo> userManager;
		private readonly SignInManager<UserDbo> signInManager;
		private readonly IEmailChecker emailChecker;
		private readonly IPasswordChecker passwordChecker;
		private readonly IUserCreator<UserDbo> userCreator;
		private readonly IConfirmEmailSender<UserDbo> confirmEmailSender;

		public AuthController(ILogger<AuthController> logger, UserManager<UserDbo> userManager, SignInManager<UserDbo> signInManager, IEmailChecker emailChecker, IPasswordChecker passwordChecker, IUserCreator<UserDbo> userCreator, IConfirmEmailSender<UserDbo> confirmEmailSender)
		{
			this.logger = logger;
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.emailChecker = emailChecker;
			this.passwordChecker = passwordChecker;
			this.userCreator = userCreator;
			this.confirmEmailSender = confirmEmailSender;
		}

		[HttpPost("SignUp")]
		public async Task<IActionResult> SignUp([FromBody] AuthData authData)
		{
			const string logMessageFormat = "Unable to sign up a new user {user}. {errors}";
			bool isEmailCorrect = emailChecker.Check(authData.Email);
			if (!isEmailCorrect)
			{
				const string emailMessage = "Email is incorrect.";
				logger.LogWarning(logMessageFormat, authData.Email, emailMessage);
				return BadRequest(new { errorMessage = emailMessage });
			}

			bool isPasswordCorrect = passwordChecker.Check(authData.Password);
			if (!isPasswordCorrect)
			{
				const string passwordMessage = "Password is incorrect.";
				logger.LogWarning(logMessageFormat, authData.Email, passwordMessage);
				return BadRequest(new { errorMessage = passwordMessage });
			}

			var user = new UserDbo() { UserName = authData.Email, Email = authData.Email };
			var result = await userCreator.CreateAsync(user, authData.Password, Roles.User);
			if (result.Succeeded)
			{
				logger.LogInformation("New user {user} just signed up", authData.Email);
				await confirmEmailSender.SendAsync(user, HttpContext);
				return StatusCode(201);
			}

			var errorMessage = string.Join(' ', result.Errors.Select(e => e.Description));
			logger.LogWarning(logMessageFormat, authData.Email, errorMessage);
			return BadRequest(new { errorMessage });
		}

		[HttpPost("SignIn")]
		public async Task<IActionResult> SignIn([FromBody] AuthData authData)
		{
			var result = await signInManager.PasswordSignInAsync(authData.Email, authData.Password, isPersistent: false, lockoutOnFailure: false);
			if (result.Succeeded)
			{
				logger.LogInformation("User {user} just signed in", authData.Email);
				return Ok();
			}

			logger.LogWarning("Unable to sign in a user {user}", authData.Email);
			return Unauthorized();
		}

		[Authorize]
		[HttpPost("SignOut")]
		public async Task<IActionResult> SignOutPost()
		{
			var name = User.Identity.Name;
			await signInManager.SignOutAsync();
			logger.LogInformation("User {user} just signed out", name);
			return Ok();
		}

		[HttpGet("ConfirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				logger.LogWarning("Email confirmation warning: a user with id {id} not found", userId);
				return NotFound();
			}

			var result = await userManager.ConfirmEmailAsync(user, code);
			if (result.Succeeded)
			{
				logger.LogInformation("A user {user} has confirmed own email", user.Email);
				HttpContext.Response.ContentType = "text/html; charset=utf-8";
				return Content("Email confirmed!");
			}

			var errorMessage = string.Join(' ', result.Errors.Select(e => e.Description));
			logger.LogWarning("Email {email} confirmation failed: {errors}", user.Email, errorMessage);
			return BadRequest(new { errorMessage });
		}
	}
}
