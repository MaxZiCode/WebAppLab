using MailKit;

using Microsoft.Extensions.Options;

using MimeKit;

using System.Threading.Tasks;

using WebAppLab.Infrastructure;

namespace WebAppLab.Services
{
	public class EmailSender : IEmailSender
	{
		private readonly MailTransport mail;
		private readonly SmtpSettings smtpSettings;

		public EmailSender(MailTransport mail, IOptions<SmtpSettings> smtpSettings)
		{
			this.mail = mail;
			mail.ServerCertificateValidationCallback = (s, c, h, e) => true;

			this.smtpSettings = smtpSettings.Value;
		}

		public async Task SendAsync(string toEmail, string subject, string body)
		{
			var message = new MimeMessage()
			{
				Subject = subject,
				Body = new TextPart("html") { Text = body }
			};
			message.From.Add(new MailboxAddress(smtpSettings.SenderName, smtpSettings.SenderEmail));
			message.To.Add(new MailboxAddress(toEmail, toEmail));

			await mail.ConnectAsync(smtpSettings.Server, smtpSettings.Port, smtpSettings.SecureSocketOptions);
			await mail.AuthenticateAsync(smtpSettings.UserName, smtpSettings.Password);
			await mail.SendAsync(message);
			await mail.DisconnectAsync(quit: true);
		}
	}
}
