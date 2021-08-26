using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using System;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Data.Models;
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
		private readonly IMemoryCache cache;

		public UserChanger(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager, IMemoryCache cache)
		{
			this.context = context;
			this.mapper = mapper;
			this.userManager = userManager;
			this.cache = cache;
		}

		public async Task<UserViewModel> GetUserAsync(string userId)
		{
			return await cache.GetOrCreateAsync(CacheKeys.User, async entry =>
			{
				entry.SlidingExpiration = TimeSpan.FromDays(1);

				var userDbo = await GetUserFromDatabaseAsync(userId);
				var user = mapper.Map<UserViewModel>(userDbo);
				return user;
			});
		}

		public async Task<ValueResult<UserViewModel>> ChangeAsync(string userId, UserViewModel user)
		{
			var userDbo = await GetUserFromDatabaseAsync(userId, track: true);

			bool isEmailCorrect = CheckUtility.CheckEmail(user.Email);
			if (!isEmailCorrect)
				return ValueResult<UserViewModel>.FromFail("Email is incorrect");

			bool isPhoneNumberCorrect = CheckUtility.CheckPhoneNumber(user.PhoneNumber);
			if (!isPhoneNumberCorrect)
				return ValueResult<UserViewModel>.FromFail("Phone number is incorrect");

			mapper.Map(user, userDbo);
			await context.SaveChangesAsync();

			cache.Remove(CacheKeys.User);

			return ValueResult<UserViewModel>.FromValue(user);
		}

		public async Task<Result> ChangePasswordAsync(string userId, string newPassword)
		{
			bool isPasswordCorrect = CheckUtility.CheckPassword(newPassword);
			if (!isPasswordCorrect)
				return Result.FromFail("Password is incorrect.");

			var userDbo = await GetUserFromDatabaseAsync(userId, track: true);
			var hashedPassword = userManager.PasswordHasher.HashPassword(userDbo, newPassword);
			userDbo.PasswordHash = hashedPassword;
			await context.SaveChangesAsync();
			return Result.Success;
		}

		private async Task<User> GetUserFromDatabaseAsync(string userId, bool track = false)
		{
			IQueryable<User> users = context.Users;

			if (!track)
				users = users.AsNoTracking();

			return await users.FirstAsync(u => u.Id == userId);
		}
	}
}
