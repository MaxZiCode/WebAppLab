using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Models;
using WebAppLab.Wrappers;

namespace WebAppLab.Services
{
	public interface IUserChanger
	{
		Task<ValueResult<UserViewModel>> ChangeAsync(ClaimsPrincipal claimsPrincipal, UserViewModel user);
		Task<Result> ChangePasswordAsync(ClaimsPrincipal claimsPrincipal, string newPassword);
		Task<UserViewModel> GetUserAsync(ClaimsPrincipal claimsPrincipal);
	}
}