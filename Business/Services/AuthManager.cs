using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Helpers;

namespace WebAppLab.Services
{
	public class AuthManager : IAuthManager
	{
		private readonly UserManager<IdentityUser> userManager;
		private readonly SignInManager<IdentityUser> signInManager;
		private readonly IConfirmEmailSender confirmEmailSender;

		public AuthManager(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfirmEmailSender confirmEmailSender)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.confirmEmailSender = confirmEmailSender;
		}

		public async Task SignUpAsync(HttpContext httpContext, string email, string password, string role)
		{
			bool isEmailCorrect = CheckUtility.CheckEmail(email);
			if (!isEmailCorrect)
			{
				throw new ArgumentException("Email is incorrect.");
			}

			bool isPasswordCorrect = CheckUtility.CheckPassword(password);
			if (!isPasswordCorrect)
			{
				throw new ArgumentException("Password is incorrect.");
			}

			var user = new IdentityUser() { Email = email, UserName = email };

			var resultCreate = await userManager.CreateAsync(user, password);
			if (!resultCreate.Succeeded)
			{
				throw new Exception($"Failed to create a new user \'{email}\': {GetErrorsMessage(resultCreate.Errors)}");
			}

			var resultRole = await userManager.AddToRoleAsync(user, role);
			if (!resultRole.Succeeded)
			{
				throw new Exception($"Failed to add role \'{role} to user \'{email}\': {GetErrorsMessage(resultRole.Errors)}");
			}

			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			await confirmEmailSender.SendAsync(httpContext, user.Email, user.Id, code);
		}

		public async Task SignInAsync(string email, string password)
		{
			var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

			if (!result.Succeeded)
			{
				throw new Exception($"Failed to sign in user \'{email}\'");
			}
		}

		public async Task SignOutAsync()
		{
			await signInManager.SignOutAsync();
		}

		public async Task ConfirmEmail(string userId, string code)
		{
			var user = await userManager.FindByIdAsync(userId);
			var result = await userManager.ConfirmEmailAsync(user, code);
			if (!result.Succeeded)
			{
				throw new Exception($"Email confirmation failed for userId \'{userId}\': {GetErrorsMessage(result.Errors)}");
			}
		}

		private static string GetErrorsMessage(IEnumerable<IdentityError> errors)
		{
			return string.Join(' ', errors.Select(e => e.Description));
		}
	}
}
