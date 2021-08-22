using AutoFixture;

using FakeItEasy;

using MailKit;

using Microsoft.Extensions.Options;

using MimeKit;

using System.Threading;
using System.Threading.Tasks;

using WebAppLab.Infrastructure;
using WebAppLab.Services;

using Xunit;

namespace WebAppLab.Tests
{
	public class EmailSenderTests
	{
		private readonly MailTransport mail;
		private readonly EmailSender emailSender;

		public EmailSenderTests()
		{
			mail = A.Fake<MailTransport>();

			var smtpSettings = Options.Create(new Fixture().Create<SmtpSettings>());
			emailSender = new EmailSender(mail, smtpSettings);
		}

		[Fact]
		public async Task Can_Send()
		{
			await emailSender.SendAsync("mail", string.Empty, string.Empty);

			A.CallTo(() => mail.SendAsync(A<MimeMessage>.Ignored, A<CancellationToken>.Ignored, A<ITransferProgress>.Ignored)).MustHaveHappened();
		}
	}
}
