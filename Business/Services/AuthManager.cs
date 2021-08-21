using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Wrappers;

namespace WebAppLab.Services
{
	public class AuthManager : IAuthManager
	{
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;
		private readonly IConfirmEmailSender confirmEmailSender;

		public AuthManager(UserManager<User> userManager, SignInManager<User> signInManager, IConfirmEmailSender confirmEmailSender)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.confirmEmailSender = confirmEmailSender;
		}

		public async Task<Result> SignUpAsync(HttpContext httpContext, string email, string password, string role)
		{
			bool isEmailCorrect = CheckUtility.CheckEmail(email);
			if (!isEmailCorrect)
				return Result.FromFail("Email is incorrect.");

			bool isPasswordCorrect = CheckUtility.CheckPassword(password);
			if (!isPasswordCorrect)
				return Result.FromFail("Password is incorrect.");

			var user = new User() { Email = email, UserName = email };

			await userManager.CreateAsync(user, password);
			await userManager.AddToRoleAsync(user, role);

			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			await confirmEmailSender.SendAsync(httpContext, user.Email, user.Id, code);
			return Result.Success;
		}

		public async Task<Result> SignInAsync(string email, string password)
		{
			var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

			if (!result.Succeeded)
				return Result.FromFail($"Failed to sign in user \'{email}\'");

			return Result.Success;
		}

		public async Task<Result> SignOutAsync()
		{
			await signInManager.SignOutAsync();
			return Result.Success;
		}

		public async Task<Result> ConfirmEmail(string userId, string code)
		{
			var user = await userManager.FindByIdAsync(userId);
			var result = await userManager.ConfirmEmailAsync(user, code);
			if (!result.Succeeded)
				return Result.FromFail($"Email confirmation failed for userId \'{userId}\': {GetErrorsMessage(result.Errors)}");

			return Result.Success;
		}

		private static string GetErrorsMessage(IEnumerable<IdentityError> errors)
		{
			return string.Join(' ', errors.Select(e => e.Description));
		}
	}
}
