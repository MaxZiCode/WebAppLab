using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using System.Threading.Tasks;

namespace WebAppLab.Services
{
	public class ConfirmEmailSender : IConfirmEmailSender
	{
		private readonly IEmailSender emailSender;
		private readonly LinkGenerator linkGenerator;

		public ConfirmEmailSender(IEmailSender emailSender, LinkGenerator linkGenerator)
		{
			this.emailSender = emailSender;
			this.linkGenerator = linkGenerator;
		}

		public async Task SendAsync(HttpContext httpContext, string email, string userId, string code)
		{
			string confirmationUrl = linkGenerator.GetUriByAction(httpContext, "ConfirmEmail", "Auth", new { userId, code });

			string subject = "WebAppLab email confirmation";
			string body = @$"<h4>Email confirmation link: <a href='{confirmationUrl}'>confirm email</a></h4>";

			await emailSender.SendAsync(email, subject, body);
		}
	}
}
