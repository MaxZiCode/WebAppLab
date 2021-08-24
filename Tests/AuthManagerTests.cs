using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Services;

using Xunit;

namespace WebAppLab.Tests
{
	public class AuthManagerTests : DbRequiredTests, IAsyncDisposable
	{
		const string validEmail = "example@example.com";
		const string invalidEmail = "invalid";
		const string validPassword = "AJNlbBjp5ZD1*!";
		const string invalidPassword = "invalid";

		private readonly IConfirmEmailSender confirmEmailSender;
		private readonly HttpContext httpContext;
		private readonly ApplicationDbContext context;
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;
		private readonly AuthManager authManager;

		public AuthManagerTests()
		{
			confirmEmailSender = A.Fake<IConfirmEmailSender>();
			httpContext = new DefaultHttpContext();
			context = new(ContextOptions);

			userManager = new(new UserStore<User>(context), null, new PasswordHasher<User>(), null, null, null, new IdentityErrorDescriber(), null, A.Fake<ILogger<UserManager<User>>>());
			userManager.RegisterTokenProvider("Default", new EmailTokenProvider<User>());

			signInManager = A.Fake<SignInManager<User>>();
			A.CallTo(() => signInManager.PasswordSignInAsync(A<string>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<bool>.Ignored)).WhenArgumentsMatch(args => (string)args[0] == validEmail && (string)args[1] == validPassword).Returns(SignInResult.Success);

			authManager = new(userManager, signInManager, confirmEmailSender);

			RoleManager<IdentityRole> roleManager = new(new RoleStore<IdentityRole>(context), null, null, null, null);
			DbFiller.EnsureRolesCreatedAsync(roleManager).Wait();
		}

		[Fact]
		public async Task SignUpAsync_ValidData_ResultSucceededUserExistsConfirmationEmailSended()
		{
			var email = validEmail;
			var password = validPassword;
			var role = Roles.User;

			var result = await authManager.SignUpAsync(httpContext, email, password, role);

			Assert.True(result.Succeeded, result.FailMessage);

			var signedUpUser = GetUser(email);
			Assert.NotNull(signedUpUser);

			A.CallTo(() => confirmEmailSender.SendAsync(httpContext, email, A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();
		}

		[Fact]
		public async Task SignUpAsync_InvalidEmail_FailMessageContainsEmailWord()
		{
			var email = invalidEmail;
			var password = validPassword;
			var role = Roles.User;

			var result = await authManager.SignUpAsync(httpContext, email, password, role);

			Assert.Contains("email", result.FailMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task SignUpAsync_InvalidPassword_FailMessageContainsPasswordWord()
		{
			var email = validEmail;
			var password = invalidPassword;
			var role = Roles.User;

			var result = await authManager.SignUpAsync(httpContext, email, password, role);

			Assert.Contains("password", result.FailMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task SignInAsync_ValidData_ResultSucceeded()
		{
			var email = validEmail;
			var password = validPassword;

			var result = await authManager.SignInAsync(email, password);

			Assert.True(result.Succeeded, result.FailMessage);
		}

		[Theory]
		[InlineData(invalidEmail, invalidPassword)]
		[InlineData(validEmail, invalidPassword)]
		[InlineData(invalidEmail, validPassword)]
		public async Task SignInAsync_InvalidDataVary_ResultNotSucceeded(string email, string password)
		{
			var result = await authManager.SignInAsync(email, password);

			Assert.False(result.Succeeded);
		}

		[Fact]
		public async Task SignOutAsync_NoData_CallsSignInManagerSignOut()
		{
			await authManager.SignOutAsync();

			A.CallTo(() => signInManager.SignOutAsync()).MustHaveHappened();
		}

		[Fact]
		public async Task ConfirmEmailAsync_ValidCode_ResultSucceededUserEmailConfirmed()
		{
			var user = new User() { UserName = validEmail, Email = validEmail };
			await userManager.CreateAsync(user, validPassword);
			user = await GetUser(user.Email);
			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

			var result = await authManager.ConfirmEmailAsync(user.Id, code);
			Assert.True(result.Succeeded, result.FailMessage);

			var assertUser = await GetUser(user.Email);
			Assert.True(assertUser.EmailConfirmed);
		}

		[Fact]
		public async Task ConfirmEmailAsync_InvalidCode_ResultNotSucceededUserEmailNotConfirmed()
		{
			var user = new User() { UserName = validEmail, Email = validEmail };
			await userManager.CreateAsync(user, validPassword);
			user = await GetUser(user.Email);
			var code = "invalid";

			var result = await authManager.ConfirmEmailAsync(user.Id, code);
			Assert.False(result.Succeeded);

			var assertUser = await GetUser(user.Email);
			Assert.False(assertUser.EmailConfirmed);
		}

		private async Task<User> GetUser(string email)
		{
			return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public ValueTask DisposeAsync()
		{
			return ((IAsyncDisposable)context).DisposeAsync();
		}
	}
}
