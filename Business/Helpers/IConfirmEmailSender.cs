using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;

namespace WebAppLab.Helpers
{
	public interface IConfirmEmailSender<in T> where T : IdentityUser
	{
		public Task SendAsync(T user, HttpContext httpContext);
	}
}
