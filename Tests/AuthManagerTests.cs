using Xunit;
using FakeItEasy;
using AutoFixture;

using Microsoft.AspNetCore.Identity;
using WebAppLab.Data.Models;
using WebAppLab.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebAppLab.Wrappers;
using System;

namespace WebAppLab.Tests
{
	public class AuthManagerTests
	{
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;
		private readonly IConfirmEmailSender confirmEmailSender;
		private readonly AuthManager authManager;

		const string validEmail = "example@example.com";
		const string invalidEmail = "invalid";
		const string validPassword = "AJNlbBjp5ZD1*!";
		const string invalidPassword = "invalid";

		string email = validEmail;
		string password = validPassword;

		public AuthManagerTests()
		{
			userManager = A.Fake<UserManager<User>>();
			signInManager = A.Fake<SignInManager<User>>();
			confirmEmailSender = A.Fake<IConfirmEmailSender>();
			authManager = new AuthManager(userManager, signInManager, confirmEmailSender);
		}

		[Fact]
		public async Task SignUp_success()
		{
			SetupSignUp();

			var result = await authManager.SignUpAsync(A.Fake<HttpContext>(), email, password, string.Empty);

			Assert.True(result.Succeeded, result.FailMessage);
		}

		[Fact]
		public async Task SignUp_incorrect_email()
		{
			SetupSignUp();
			email = invalidEmail;

			var result = await MakeSignUp();

			Assert.Contains("email", result.FailMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task SignUp_incorrect_password()
		{
			SetupSignUp();
			password = invalidPassword;

			var result = await MakeSignUp();

			Assert.Contains("password", result.FailMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task SignUp_fail_create()
		{
			SetupSignUp(failCreate: true);

			var result = await MakeSignUp();

			Assert.Contains("create", result.FailMessage);
		}

		[Fact]
		public async Task SignUp_fail_add_role()
		{
			SetupSignUp(failAddRole: true);

			var result = await MakeSignUp();

			Assert.Contains("add role", result.FailMessage);
		}

		[Fact]
		public async Task SignUp_creates_a_user()
		{
			SetupSignUp();

			await MakeSignUp();

			A.CallTo(() => userManager.CreateAsync(A<User>._, A<string>._)).MustHaveHappened();
		}

		[Fact]
		public async Task SignUp_adds_a_user_to_role()
		{
			SetupSignUp();

			await MakeSignUp();

			A.CallTo(() => userManager.AddToRoleAsync(A<User>._, A<string>._)).MustHaveHappened();
		}

		[Fact]
		public async Task SignUp_generates_email_confirmation_token()
		{
			SetupSignUp();

			await MakeSignUp();

			A.CallTo(() => userManager.GenerateEmailConfirmationTokenAsync(A<User>._)).MustHaveHappened();
		}

		[Fact]
		public async Task SignUp_sends_email_confirmation()
		{
			SetupSignUp();

			await MakeSignUp();

			A.CallTo(() => confirmEmailSender.SendAsync(A<HttpContext>._, A<string>._, A<string>._, A<string>._)).MustHaveHappened();
		}

		private void SetupSignUp(bool failCreate = false, bool failAddRole = false)
		{
			if (!failCreate)
				A.CallTo(() => userManager.CreateAsync(A<User>._, A<string>._)).Returns(IdentityResult.Success);

			if (!failAddRole)
				A.CallTo(() => userManager.AddToRoleAsync(A<User>._, A<string>._)).Returns(IdentityResult.Success);
		}

		private async Task<Result> MakeSignUp()
		{
			return await authManager.SignUpAsync(A.Fake<HttpContext>(), email, password, string.Empty);
		}

		[Fact]
		public async Task SignIn_success()
		{
			SetupSignIn();

			var result = await authManager.SignInAsync(validEmail, validPassword);

			Assert.True(result.Succeeded, result.FailMessage);
		}

		[Fact]
		public async Task SignIn_fail()
		{
			SetupSignIn(fail: true);

			var result = await authManager.SignInAsync(invalidEmail, invalidPassword);

			Assert.False(result.Succeeded);
		}

		[Fact]
		public async Task SignIn_calls_PasswordSignIn()
		{
			SetupSignIn();

			await authManager.SignInAsync(validEmail, validPassword);

			A.CallTo(() => signInManager.PasswordSignInAsync(A<string>._, A<string>._, A<bool>._, A<bool>._)).MustHaveHappened();
		}

		private void SetupSignIn(bool fail = false)
		{
			if (!fail)
				A.CallTo(() => signInManager.PasswordSignInAsync(A<string>._, A<string>._, A<bool>._, A<bool>._)).Returns(SignInResult.Success);
		}

		[Fact]
		public async Task SignOut_calls_SignOut()
		{
			await authManager.SignOutAsync();

			A.CallTo(() => signInManager.SignOutAsync()).MustHaveHappened();
		}

		[Fact]
		public async Task ConfirmEmail_success()
		{
			SetupConfirmEmail();

			var result = await authManager.ConfirmEmail(string.Empty, string.Empty);

			Assert.True(result.Succeeded, result.FailMessage);
		}

		[Fact]
		public async Task ConfirmEmail_fail()
		{
			SetupConfirmEmail(fail: true);

			var result = await authManager.ConfirmEmail(string.Empty, string.Empty);

			Assert.False(result.Succeeded);
		}

		[Fact]
		public async Task ConfirmEmail_calls_ConfirmEmail()
		{
			SetupConfirmEmail();

			var result = await authManager.ConfirmEmail(string.Empty, string.Empty);

			A.CallTo(() => userManager.ConfirmEmailAsync(A<User>._, A<string>._)).MustHaveHappened();
		}

		private void SetupConfirmEmail(bool fail = false)
		{
			if (!fail)
				A.CallTo(() => userManager.ConfirmEmailAsync(A<User>._, A<string>._)).Returns(IdentityResult.Success);
		}
	}
}
