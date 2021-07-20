using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using MimeKit;

using System.Threading.Tasks;

using WebAppLab.Infrastructure;

namespace WebAppLab.Services
{
	public class EmailSender : IEmailSender
	{
		private readonly SmtpSettings smtpSettings;

		public EmailSender(IOptions<SmtpSettings> smtpSettings)
		{
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

			using var client = new SmtpClient();
			client.ServerCertificateValidationCallback = (s, c, h, e) => true;
			await client.ConnectAsync(smtpSettings.Server, smtpSettings.Port, smtpSettings.SecureSocketOptions);
			await client.AuthenticateAsync(smtpSettings.UserName, smtpSettings.Password);
			await client.SendAsync(message);
			await client.DisconnectAsync(quit: true);
		}
	}
}
