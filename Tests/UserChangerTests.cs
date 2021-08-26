using AutoFixture;

using AutoMapper;

using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Models;
using WebAppLab.Services;

using Xunit;

namespace WebAppLab.Tests
{
	public class UserChangerTests : DbRequiredTests, IAsyncDisposable
	{
		private readonly IMapper mapper;
		private readonly IMemoryCache cache;
		private readonly ApplicationDbContext context;
		private readonly UserManager<User> userManager;
		private readonly UserChanger userChanger;
		private readonly User dbUser = new()
		{
			Id = Guid.NewGuid().ToString(),
			Email = "dbEmail@emaple.com",
			PhoneNumber = "375123456789",
			PasswordHash = Guid.NewGuid().ToString()
		};

		public UserChangerTests()
		{
			var mapperConfig = new MapperConfiguration(config => config.CreateMap<User, UserViewModel>().ReverseMap());
			mapper = new Mapper(mapperConfig);

			cache = new MemoryCache(new MemoryCacheOptions());

			context = new ApplicationDbContext(ContextOptions);

			userManager = new(new UserStore<User>(context), null, new PasswordHasher<User>(), null, null, null, new IdentityErrorDescriber(), null, A.Fake<ILogger<UserManager<User>>>());
			userChanger = new(context, mapper, userManager, cache);

			userManager.CreateAsync(dbUser).Wait();
		}

		[Fact]
		public async Task GetUserAsync_ExistingUserId_ReturnsUserAndCacheSetted()
		{
			var actualUser = await userChanger.GetUserAsync(dbUser.Id);

			Assert.Equal(dbUser.Email, actualUser.Email);

			UserViewModel cacheUser = cache.Get<UserViewModel>(CacheKeys.User);
			Assert.Equal(dbUser.Email, cacheUser.Email);
		}

		[Fact]
		public async Task GetUserAsync_ExistingUserSettedInCache_ReturnsUserFromCache()
		{
			UserViewModel cachedUser = new() { Email = "cacheEmail@example.com" };
			cache.Set(CacheKeys.User, cachedUser);

			var actualUser = await userChanger.GetUserAsync(dbUser.Id);

			Assert.Equal(cachedUser.Email, actualUser.Email);
		}

		[Fact]
		public async Task ChangeAsync_UserViewModelWithValidEmailAndPhone_RetunrsChangedUserAndChangesInDbAndCacheHasDeleted()
		{
			UserViewModel changedUser = mapper.Map<UserViewModel>(dbUser);
			changedUser.Email = "changed@example.com";
			changedUser.PhoneNumber = "573123456789";
			cache.Set(CacheKeys.User, changedUser);

			var result = await userChanger.ChangeAsync(dbUser.Id, changedUser);

			Assert.True(result.Succeeded);

			Assert.Equal(changedUser.Email, result.Data.Email);
			Assert.Equal(changedUser.PhoneNumber, result.Data.PhoneNumber);

			var changedDbUser = await context.Users.FindAsync(dbUser.Id);
			Assert.Equal(changedUser.Email, changedDbUser.Email);
			Assert.Equal(changedUser.PhoneNumber, changedDbUser.PhoneNumber);

			var cachedUser = cache.Get(CacheKeys.User);
			Assert.Null(cachedUser);
		}

		[Fact]
		public async Task ChangeAsync_UserViewModelWithInvalidEmail_RetunrsResultFailMessageContainsEmailWordAndContextNotChanged()
		{
			UserViewModel changedUser = mapper.Map<UserViewModel>(dbUser);
			changedUser.Email = "changed";
			bool changed = false;
			context.ChangeTracker.StateChanged += (o, e) => changed = true;

			var result = await userChanger.ChangeAsync(dbUser.Id, changedUser);

			Assert.Contains("email", result.FailMessage, StringComparison.OrdinalIgnoreCase);
			Assert.False(changed);
		}

		[Fact]
		public async Task ChangeAsync_UserViewModelWithInvalidPhone_RetunrsResultFailMessageContainsPhoneWordAndContextNotChanged()
		{
			UserViewModel changedUser = mapper.Map<UserViewModel>(dbUser);
			changedUser.PhoneNumber = "911";
			bool changed = false;
			context.ChangeTracker.StateChanged += (o, e) => changed = true;

			var result = await userChanger.ChangeAsync(dbUser.Id, changedUser);

			Assert.Contains("phone", result.FailMessage, StringComparison.OrdinalIgnoreCase);
			Assert.False(changed);
		}

		[Fact]
		public async Task ChangePasswordAsync_NewValidPassword_RetunrsResultSuccessedAndContextChanged()
		{
			string passwordHashBefore = dbUser.PasswordHash;
			string newPassword = "uac555444ALLL!***";
			bool changed = false;
			context.ChangeTracker.StateChanged += (o, e) => changed = true;

			var result = await userChanger.ChangePasswordAsync(dbUser.Id, newPassword);

			Assert.True(result.Succeeded);
			Assert.True(changed);
		}

		[Fact]
		public async Task ChangePasswordAsync_NewInvalidPassword_RetunrsResultFailMessageContainsPasswordWordAndContextNotChanged()
		{
			string passwordHashBefore = dbUser.PasswordHash;
			string newPassword = "fail";
			bool changed = false;
			context.ChangeTracker.StateChanged += (o, e) => changed = true;

			var result = await userChanger.ChangePasswordAsync(dbUser.Id, newPassword);

			Assert.Contains("password", result.FailMessage, StringComparison.OrdinalIgnoreCase);
			Assert.False(changed);
		}

		public ValueTask DisposeAsync()
		{
			return ((IAsyncDisposable)context).DisposeAsync();
		}
	}
}
