using System.Threading.Tasks;

using WebAppLab.Models;
using WebAppLab.Wrappers;

namespace WebAppLab.Services
{
	public interface IUserChanger
	{
		Task<ValueResult<UserViewModel>> ChangeAsync(string userId, UserViewModel user);
		Task<Result> ChangePasswordAsync(string userId, string newPassword);
		Task<UserViewModel> GetUserAsync(string userId);
	}
}