using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Models;

namespace WebAppLab.Services
{
	public interface IUserChanger
	{
		Task<UserChangeResult> ChangeAsync(ClaimsPrincipal claimsPrincipal, User user);
		Task<AuthResult> ChangePasswordAsync(ClaimsPrincipal claimsPrincipal, string newPassword);
		Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal);
	}
}