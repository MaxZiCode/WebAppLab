using MailKit.Net.Smtp;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

using System.Threading.Tasks;

namespace WebAppLab
{
	public class DefaultEmailSender : IEmailSender
	{
		private readonly SmtpSettings smtpSettings;
		private readonly ILogger<DefaultEmailSender> logger;

		public DefaultEmailSender(IOptions<SmtpSettings> smtpSettings, ILogger<DefaultEmailSender> logger)
		{
			this.smtpSettings = smtpSettings.Value;
			this.logger = logger;
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

			logger.LogInformation("Email \'{subject}\' has sended to {email}.", subject, toEmail);
		}
	}
}
