using Microsoft.AspNetCore.Identity;

using System.Linq;
using System.Threading.Tasks;

namespace WebAppLab.Helpers
{
	public class DefaultUserCreator<T> : IUserCreator<T> where T : IdentityUser
	{
		private readonly UserManager<T> userManager;

		public DefaultUserCreator(UserManager<T> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<IdentityResult> CreateAsync(T user, string password, string role)
		{
			var resultCreate = await userManager.CreateAsync(user, password);
			var resultRole = await userManager.AddToRoleAsync(user, role);

			if (resultCreate.Succeeded && resultRole.Succeeded)
				return IdentityResult.Success;

			return IdentityResult.Failed(resultCreate.Errors.Concat(resultRole.Errors).ToArray());
		}
	}
}
