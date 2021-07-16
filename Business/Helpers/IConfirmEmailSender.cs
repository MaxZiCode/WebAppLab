using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;

namespace WebAppLab.Helpers
{
	public interface IConfirmEmailSender
	{
		public Task SendAsync(IdentityUser user, HttpContext httpContext);
	}
}
