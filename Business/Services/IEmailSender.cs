using System.Threading.Tasks;

namespace WebAppLab.Services
{
	public interface IEmailSender
	{
		Task SendAsync(string toAddress, string subject, string body);
	}
}
