using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

using System.Threading.Tasks;

namespace WebAppLab.Helpers
{
	public class DefaultConfirmEmailSender<T> : IConfirmEmailSender<T> where T : IdentityUser
	{
		private readonly UserManager<T> userManager;
		private readonly IEmailSender emailSender;
		private readonly LinkGenerator linkGenerator;

		public DefaultConfirmEmailSender(UserManager<T> userManager, IEmailSender emailSender, LinkGenerator linkGenerator)
		{
			this.userManager = userManager;
			this.emailSender = emailSender;
			this.linkGenerator = linkGenerator;
		}

		public async Task SendAsync(T user, HttpContext httpContext)
		{
			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			var confirmationUrl = linkGenerator.GetUriByAction(httpContext, "ConfirmEmail", "Auth", new { userId = user.Id, code });
			await emailSender.SendAsync(user.Email, "WebAppLab email confirmation", @$"<h4>Email confirmation link: <a href='{confirmationUrl}'>confirm email</a></h4>");
		}
	}
}
