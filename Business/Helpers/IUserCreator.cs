using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;

namespace WebAppLab.Helpers
{
	public interface IUserCreator
	{
		public Task<IdentityResult> CreateAsync(IdentityUser user, string password, string role);
	}
}
