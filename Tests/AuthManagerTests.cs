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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using WebAppLab.Data;

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
		public async Task Can_SignUp()
		{
			SetupSignUp();

			var result = await authManager.SignUpAsync(A.Fake<HttpContext>(), email, password, string.Empty);

			A.CallTo(() => userManager.CreateAsync(A<User>.Ignored, A<string>.Ignored)).MustHaveHappened();
			A.CallTo(() => userManager.AddToRoleAsync(A<User>.Ignored, A<string>.Ignored)).MustHaveHappened();
			A.CallTo(() => confirmEmailSender.SendAsync(A<HttpContext>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();
			Assert.True(result.Succeeded, result.FailMessage);
		}

		[Fact]
		public async Task Cannot_SignUp_incorrect_email()
		{
			SetupSignUp();
			email = invalidEmail;

			var result = await MakeSignUp();

			Assert.Contains("email", result.FailMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task Cannot_SignUp_incorrect_password()
		{
			SetupSignUp();
			password = invalidPassword;

			var result = await MakeSignUp();

			Assert.Contains("password", result.FailMessage, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task Cannot_SignUp_fail_create()
		{
			SetupSignUp(failCreate: true);

			var result = await MakeSignUp();

			Assert.Contains("create", result.FailMessage);
		}

		[Fact]
		public async Task Cannot_SignUp_fail_add_role()
		{
			SetupSignUp(failAddRole: true);

			var result = await MakeSignUp();

			Assert.Contains("role", result.FailMessage);
		}

		private void SetupSignUp(bool failCreate = false, bool failAddRole = false)
		{
			if (!failCreate)
				A.CallTo(() => userManager.CreateAsync(A<User>.Ignored, A<string>._)).Returns(IdentityResult.Success);

			if (!failAddRole)
				A.CallTo(() => userManager.AddToRoleAsync(A<User>.Ignored, A<string>._)).Returns(IdentityResult.Success);
		}

		private async Task<Result> MakeSignUp()
		{
			return await authManager.SignUpAsync(A.Fake<HttpContext>(), email, password, string.Empty);
		}

		[Fact]
		public async Task Can_SignIn()
		{
			SetupSignIn();

			var result = await authManager.SignInAsync(validEmail, validPassword);

			A.CallTo(() => signInManager.PasswordSignInAsync(A<string>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<bool>.Ignored));
			Assert.True(result.Succeeded, result.FailMessage);
		}

		[Fact]
		public async Task Cannot_SignIn()
		{
			SetupSignIn(fail: true);

			var result = await authManager.SignInAsync(invalidEmail, invalidPassword);

			Assert.False(result.Succeeded);
		}

		private void SetupSignIn(bool fail = false)
		{
			if (!fail)
				A.CallTo(() => signInManager.PasswordSignInAsync(A<string>._, A<string>._, A<bool>._, A<bool>._)).Returns(SignInResult.Success);
		}

		[Fact]
		public async Task Can_SignOut()
		{
			var result = await authManager.SignOutAsync();

			A.CallTo(() => signInManager.SignOutAsync()).MustHaveHappened();
		}

		[Fact]
		public async Task Can_ConfirmEmail()
		{
			SetupConfirmEmail();

			var result = await authManager.ConfirmEmail(string.Empty, string.Empty);

			A.CallTo(() => userManager.ConfirmEmailAsync(A<User>.Ignored, A<string>.Ignored)).MustHaveHappened();
			Assert.True(result.Succeeded, result.FailMessage);
		}

		[Fact]
		public async Task Cannot_ConfirmEmail_incorrect_code()
		{
			SetupConfirmEmail(fail: true);

			var result = await authManager.ConfirmEmail(string.Empty, string.Empty);

			Assert.False(result.Succeeded);
		}

		private void SetupConfirmEmail(bool fail = false)
		{
			if (!fail)
				A.CallTo(() => userManager.ConfirmEmailAsync(A<User>._, A<string>._)).Returns(IdentityResult.Success);
		}
	}
}
