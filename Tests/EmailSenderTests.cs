using AutoFixture;

using FakeItEasy;

using MailKit;

using Microsoft.Extensions.Options;

using MimeKit;

using System;
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
		public async Task SendAsync_AnyData_CallsMailTransportSendAsync()
		{
			string email = Guid.NewGuid().ToString();
			string subject = Guid.NewGuid().ToString();
			string body = Guid.NewGuid().ToString();

			await emailSender.SendAsync(email, subject, body);

			A.CallTo(() => mail.SendAsync(A<MimeMessage>.Ignored, A<CancellationToken>.Ignored, A<ITransferProgress>.Ignored)).MustHaveHappened();
		}
	}
}
