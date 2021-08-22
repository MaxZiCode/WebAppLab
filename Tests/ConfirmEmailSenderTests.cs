using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using System.Threading.Tasks;

using WebAppLab.Services;

using Xunit;

namespace WebAppLab.Tests
{
	public class ConfirmEmailSenderTests
	{
		private readonly IEmailSender emailSender;
		private readonly LinkGenerator linkGenerator;
		private readonly ConfirmEmailSender confirmEmailSender;

		public ConfirmEmailSenderTests()
		{
			emailSender = A.Fake<IEmailSender>();
			linkGenerator = A.Fake<LinkGenerator>();
			confirmEmailSender = new ConfirmEmailSender(emailSender, linkGenerator);
		}

		[Fact]
		public async Task Can_Send()
		{
			var email = "email";
			var userId = "1";
			var code = "code";
			await confirmEmailSender.SendAsync(A.Fake<HttpContext>(), email, userId, code);

			A.CallTo(() => emailSender.SendAsync(email, A<string>.Ignored, A<string>.Ignored));
		}
	}
}
