using System.Threading.Tasks;

namespace WebAppLab.Helpers
{
	public interface IEmailSender
	{
		public Task SendAsync(string toAddress, string subject, string body);
	}
}
