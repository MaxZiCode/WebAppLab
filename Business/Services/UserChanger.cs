using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Extensions;
using WebAppLab.Helpers;
using WebAppLab.Models;
using WebAppLab.Wrappers;

namespace WebAppLab.Services
{
	public class UserChanger : IUserChanger
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly UserManager<User> userManager;

		public UserChanger(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager)
		{
			this.context = context;
			this.mapper = mapper;
			this.userManager = userManager;
		}

		public async Task<UserViewModel> GetUserAsync(ClaimsPrincipal claimsPrincipal)
		{
			var userDbo = await GetUserFromDatabaseAsync(claimsPrincipal);
			var user = mapper.Map<UserViewModel>(userDbo);
			return user;
		}

		public async Task<ValueResult<UserViewModel>> ChangeAsync(ClaimsPrincipal claimsPrincipal, UserViewModel user)
		{
			var userDbo = await GetUserFromDatabaseAsync(claimsPrincipal);

			bool isEmailCorrect = CheckUtility.CheckEmail(user.Email);
			if (!isEmailCorrect)
				return ValueResult<UserViewModel>.FromFail("Email is incorrect");

			bool isPhoneNumberCorrect = CheckUtility.CheckPhoneNumber(user.PhoneNumber);
			if (!isPhoneNumberCorrect)
				return ValueResult<UserViewModel>.FromFail("Phone number is incorrect");

			mapper.Map(user, userDbo);
			context.Users.Update(userDbo);
			await context.SaveChangesAsync();

			return ValueResult<UserViewModel>.FromValue(user);
		}

		public async Task<Result> ChangePasswordAsync(ClaimsPrincipal claimsPrincipal, string newPassword)
		{
			bool isPasswordCorrect = CheckUtility.CheckPassword(newPassword);
			if (!isPasswordCorrect)
				return Result.FromFail("Password is incorrect.");

			var userDbo = await GetUserFromDatabaseAsync(claimsPrincipal);
			var hashedPassword = userManager.PasswordHasher.HashPassword(userDbo, newPassword);
			userDbo.PasswordHash = hashedPassword;
			context.Users.Update(userDbo);
			await context.SaveChangesAsync();
			return Result.Success;
		}

		private async Task<User> GetUserFromDatabaseAsync(ClaimsPrincipal claimsPrincipal)
		{
			var id = claimsPrincipal.GetId();
			var userDbo = await context.Users.AsNoTracking().FirstAsync(u => u.Id == id);
			return userDbo;
		}
	}
}
