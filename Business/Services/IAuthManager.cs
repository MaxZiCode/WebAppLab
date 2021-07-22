using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;

using WebAppLab.Models;

namespace WebAppLab.Services
{
	public interface IAuthManager
	{
		Task<Result> ConfirmEmail(string userId, string code);
		Task<Result> SignInAsync(string email, string password);
		Task<Result> SignOutAsync();
		Task<Result> SignUpAsync(HttpContext httpContext, string email, string password, string role);
	}
}