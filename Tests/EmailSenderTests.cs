using FakeItEasy;
using AutoFixture;

using MailKit;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using MimeKit;

using System;
using System.Threading;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Infrastructure;
using WebAppLab.Services;
using WebAppLab.Wrappers;

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
