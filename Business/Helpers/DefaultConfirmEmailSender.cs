using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

using System.Threading.Tasks;

namespace WebAppLab.Helpers
{
	public class DefaultConfirmEmailSender : IConfirmEmailSender
	{
		private readonly UserManager<IdentityUser> userManager;
		private readonly IEmailSender emailSender;
		private readonly LinkGenerator linkGenerator;

		public DefaultConfirmEmailSender(UserManager<IdentityUser> userManager, IEmailSender emailSender, LinkGenerator linkGenerator)
		{
			this.userManager = userManager;
			this.emailSender = emailSender;
			this.linkGenerator = linkGenerator;
		}

		public async Task SendAsync(IdentityUser user, HttpContext httpContext)
		{
			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			var confirmationUrl = linkGenerator.GetUriByAction(httpContext, "ConfirmEmail", "Auth", new { userId = user.Id, code });
			await emailSender.SendAsync(user.Email, "WebAppLab email confirmation", @$"<h4>Email confirmation link: <a href='{confirmationUrl}'>confirm email</a></h4>");
		}
	}
}
