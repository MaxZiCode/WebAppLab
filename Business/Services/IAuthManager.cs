using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;

namespace WebAppLab.Services
{
	public interface IAuthManager
	{
		Task ConfirmEmail(string userId, string code);
		Task SignInAsync(string email, string password);
		Task SignOutAsync();
		Task SignUpAsync(HttpContext httpContext, string email, string password, string role);
	}
}