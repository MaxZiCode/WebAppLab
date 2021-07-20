using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;

namespace WebAppLab.Services
{
	public interface IConfirmEmailSender
	{
		Task SendAsync(HttpContext httpContext, string email, string userId, string code);
	}
}
