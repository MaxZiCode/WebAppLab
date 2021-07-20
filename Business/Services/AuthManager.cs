using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Models;

namespace WebAppLab.Services
{
	public class AuthManager : IAuthManager
	{
		private readonly UserManager<UserDbo> userManager;
		private readonly SignInManager<UserDbo> signInManager;
		private readonly IConfirmEmailSender confirmEmailSender;

		public AuthManager(UserManager<UserDbo> userManager, SignInManager<UserDbo> signInManager, IConfirmEmailSender confirmEmailSender)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.confirmEmailSender = confirmEmailSender;
		}

		public async Task<AuthResult> SignUpAsync(HttpContext httpContext, string email, string password, string role)
		{
			bool isEmailCorrect = CheckUtility.CheckEmail(email);
			if (!isEmailCorrect)
				return AuthResult.FromFail("Email is incorrect.");

			bool isPasswordCorrect = CheckUtility.CheckPassword(password);
			if (!isPasswordCorrect)
				return AuthResult.FromFail("Password is incorrect.");

			var user = new UserDbo() { Email = email, UserName = email };

			var resultCreate = await userManager.CreateAsync(user, password);
			if (!resultCreate.Succeeded)
				return AuthResult.FromFail($"Failed to create a new user \'{email}\': {GetErrorsMessage(resultCreate.Errors)}");

			var resultRole = await userManager.AddToRoleAsync(user, role);
			if (!resultRole.Succeeded)
				return AuthResult.FromFail($"Failed to add role \'{role} to user \'{email}\': {GetErrorsMessage(resultRole.Errors)}");

			var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			await confirmEmailSender.SendAsync(httpContext, user.Email, user.Id, code);
			return AuthResult.Success;
		}

		public async Task<AuthResult> SignInAsync(string email, string password)
		{
			var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

			if (!result.Succeeded)
				return AuthResult.FromFail($"Failed to sign in user \'{email}\'");

			return AuthResult.Success;
		}

		public async Task<AuthResult> SignOutAsync()
		{
			await signInManager.SignOutAsync();
			return AuthResult.Success;
		}

		public async Task<AuthResult> ConfirmEmail(string userId, string code)
		{
			var user = await userManager.FindByIdAsync(userId);
			var result = await userManager.ConfirmEmailAsync(user, code);
			if (!result.Succeeded)
				return AuthResult.FromFail($"Email confirmation failed for userId \'{userId}\': {GetErrorsMessage(result.Errors)}");

			return AuthResult.Success;
		}

		private static string GetErrorsMessage(IEnumerable<IdentityError> errors)
		{
			return string.Join(' ', errors.Select(e => e.Description));
		}
	}
}
