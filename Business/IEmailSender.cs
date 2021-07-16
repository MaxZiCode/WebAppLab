using System.Threading.Tasks;

namespace WebAppLab
{
	public interface IEmailSender
	{
		public Task SendAsync(string toAddress, string subject, string body);
	}
}
