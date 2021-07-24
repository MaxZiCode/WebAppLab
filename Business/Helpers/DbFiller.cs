using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Data.Models;
using WebAppLab.Models;

namespace WebAppLab.Helpers
{
	public static class DbFiller
	{
		public static async Task EnsureRolesCreatedAsync(RoleManager<IdentityRole> roleManager)
		{
			var roles = typeof(Roles).GetFields().Select(f => f.GetValue(null));
			foreach (string role in roles)
			{
				var isRoleExists = await roleManager.RoleExistsAsync(role);
				if (!isRoleExists)
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}
		}

		public static async Task EnsureAdminCreatedAsync(UserManager<User> userManager, IOptions<AuthData> adminOptions)
		{
			var adminData = adminOptions.Value;

			var user = await userManager.FindByEmailAsync(adminData.Email);
			if (user == null)
			{
				var admin = new User() { Email = adminData.Email, UserName = adminData.Email };
				await userManager.CreateAsync(admin, adminData.Password);
				await userManager.AddToRoleAsync(admin, Roles.Admin);
			}
		}
	}
}
