using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ILogger<AuthController> logger;
		private readonly UserManager<IdentityUser> userManager;
		private readonly SignInManager<IdentityUser> signInManager;
		private readonly IEmailSender emailSender;

		public AuthController(ILogger<AuthController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
		{
			this.logger = logger;
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.emailSender = emailSender;
		}

		[HttpPost("SignUp")]
		public async Task<IActionResult> SignUp([FromBody] AuthData authData)
		{
			bool isEmailCorrect = Regex.IsMatch(
				authData.Email,
				// Regex source: https://ihateregex.io/expr/email/.
				@"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+",
				RegexOptions.Compiled | RegexOptions.CultureInvariant,
				TimeSpan.FromMilliseconds(128));

			const string logMessageFormat = "Unable to sign up a new user {user}. {errors}";
			if (!isEmailCorrect)
			{
				const string emailMessage = "Email is incorrect.";
				logger.LogWarning(logMessageFormat, authData.Email, emailMessage);
				return BadRequest(new { errorMessage = emailMessage });
			}

			bool isPasswordCorrect = Regex.IsMatch(
				authData.Password,
				// Regex source: https://ihateregex.io/expr/password/.
				@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,24}$",
				RegexOptions.Compiled | RegexOptions.CultureInvariant,
				TimeSpan.FromMilliseconds(128));

			if (!isPasswordCorrect)
			{
				const string passwordMessage = "Password is incorrect.";
				logger.LogWarning(logMessageFormat, authData.Email, passwordMessage);
				return BadRequest(new { errorMessage = passwordMessage });
			}

			var user = new IdentityUser { Email = authData.Email, UserName = authData.Email };
			var result = await userManager.CreateAsync(user, authData.Password);
			if (result.Succeeded)
			{
				logger.LogInformation("New user {user} just signed up", authData.Email);
				await SendEmailConfirmationMessageAsync(user);
				return StatusCode(201);
			}

			var errorMessage = string.Join(' ', result.Errors.Select(e => e.Description));
			logger.LogWarning(logMessageFormat, authData.Email, errorMessage);
			return BadRequest(new { errorMessage });
		}

		private async Task SendEmailConfirmationMessageAsync(IdentityUser user)
		{
			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			var confirmationUrl = Url.Action(nameof(ConfirmEmail), "Auth", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
			await emailSender.SendAsync(user.Email, "WebAppLab email confirmation", @$"<h4>Email confirmation link: <a href='{confirmationUrl}'>confirm email</a></h4>");
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
