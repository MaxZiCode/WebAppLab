using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Models;

namespace WebAppLab.Services
{
	public class UserChanger : IUserChanger
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly UserManager<UserDbo> userManager;

		public UserChanger(ApplicationDbContext context, IMapper mapper, UserManager<UserDbo> userManager)
		{
			this.context = context;
			this.mapper = mapper;
			this.userManager = userManager;
		}

		public async Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal)
		{
			var userDbo = await GetUserFromDatabaseAsync(claimsPrincipal);
			var user = mapper.Map<User>(userDbo);
			return user;
		}

		public async Task<UserChangeResult> ChangeAsync(ClaimsPrincipal claimsPrincipal, User user)
		{
			var userDboTask = GetUserFromDatabaseAsync(claimsPrincipal);

			bool isEmailCorrect = CheckUtility.CheckEmail(user.Email);
			if (!isEmailCorrect)
				return UserChangeResult.FromError("Email is incorrect");

			bool isPhoneNumberCorrect = CheckUtility.CheckPhoneNumber(user.PhoneNumber);
			if (!isPhoneNumberCorrect)
				return UserChangeResult.FromError("Phone number is incorrect");

			var userDbo = await userDboTask;
			mapper.Map(user, userDbo);
			await context.SaveChangesAsync();

			var changedUser = await GetUserAsync(claimsPrincipal);
			return UserChangeResult.FromUser(changedUser);
		}

		public async Task<AuthResult> ChangePasswordAsync(ClaimsPrincipal claimsPrincipal, string newPassword)
		{
			bool isPasswordCorrect = CheckUtility.CheckPassword(newPassword);
			if (!isPasswordCorrect)
				return AuthResult.FromFail("Password is incorrect.");

			var userDbo = await GetUserFromDatabaseAsync(claimsPrincipal);
			var hashedPassword = userManager.PasswordHasher.HashPassword(userDbo, newPassword);
			userDbo.PasswordHash = hashedPassword;
			await context.SaveChangesAsync();
			return AuthResult.Success;
		}

		private async Task<UserDbo> GetUserFromDatabaseAsync(ClaimsPrincipal claimsPrincipal)
		{
			var id = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var userDbo = await context.Users.FirstAsync(u => u.Id == id);
			return userDbo;
		}
	}
}
