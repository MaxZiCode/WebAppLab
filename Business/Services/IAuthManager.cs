using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;

using WebAppLab.Models;

namespace WebAppLab.Services
{
	public interface IAuthManager
	{
		Task<AuthResult> ConfirmEmail(string userId, string code);
		Task<AuthResult> SignInAsync(string email, string password);
		Task<AuthResult> SignOutAsync();
		Task<AuthResult> SignUpAsync(HttpContext httpContext, string email, string password, string role);
	}
}