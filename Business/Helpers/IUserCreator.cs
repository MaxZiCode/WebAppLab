using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;

namespace WebAppLab.Helpers
{
	public interface IUserCreator<in T> where T : IdentityUser
	{
		public Task<IdentityResult> CreateAsync(T user, string password, string role);
	}
}
