using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppLab.Controllers
{
	// TODO: Controller or controllerbase?
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
			var user = new IdentityUser { Email = authData.Email, UserName = authData.Email };

			var result = await userManager.CreateAsync(user, authData.Password);
			if (result.Succeeded)
			{
				logger.LogInformation($"User {authData.Email} just registered");
				return StatusCode(201);
			}

			var errorMessage = new StringBuilder("Unable to sign up a new user. ")
				.AppendJoin(" ", result.Errors.Select(e => e.Description)).ToString();
			logger.LogWarning(errorMessage);
			return BadRequest(new { errorMessage });
		}
	}
}
