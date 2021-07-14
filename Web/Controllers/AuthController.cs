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

		[HttpPost("signUp")]
		public async Task<IActionResult> SignUp([FromBody] AuthData authData)
		{
			const string logMessageFormat = "Unable to sign up a new user {user}. {errors}";
			const string emailMessage = "Email is incorrect.";
			const string passwordMessage = "Password is incorrect.";

			bool isEmailCorrect = Regex.IsMatch(
				authData.Email,
				// Regex source: https://ihateregex.io/expr/email/.
				@"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+",
				RegexOptions.Compiled | RegexOptions.CultureInvariant, 
				TimeSpan.FromMilliseconds(128));

			if (!isEmailCorrect)
			{
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
				logger.LogWarning(logMessageFormat, authData.Email, passwordMessage);
				return BadRequest(new { errorMessage = passwordMessage });
			}

			var user = new IdentityUser { Email = authData.Email, UserName = authData.Email };

			var result = await userManager.CreateAsync(user, authData.Password);
			if (result.Succeeded)
			{
				logger.LogInformation($"User {authData.Email} just registered");
				return StatusCode(201);
			}

			var errorMessage = string.Join(' ', result.Errors.Select(e => e.Description));
			logger.LogWarning(logMessageFormat, authData.Email, errorMessage);
			return BadRequest(new { errorMessage });
		}
	}
}
