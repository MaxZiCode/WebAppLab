using System;

using Xunit;
using FakeItEasy;
using AutoFixture;
using WebAppLab.Services;
using WebAppLab.Controllers;
using System.Security.Claims;
using WebAppLab.Models;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebAppLab.Wrappers;
using Microsoft.AspNetCore.JsonPatch;

namespace WebAppLab.Tests
{
	public class UserControllerTests
	{
		private readonly IUserChanger userChanger;
		private readonly UserController userController;

		public UserControllerTests()
		{
			userChanger = A.Fake<IUserChanger>();
			userController = new(userChanger);
			userController.ControllerContext = new()
			{
				HttpContext = new DefaultHttpContext
				{
					User = new()
				}
			};
		}

		[Fact]
		public async Task GetUser_StatusCode200()
		{
			A.CallTo(() => userChanger.GetUserAsync(A<ClaimsPrincipal>.Ignored)).Returns(A.Fake<UserViewModel>());
			
			var actionResult = await userController.GetUser();

			Assert.IsType<OkObjectResult>(actionResult.Result);
		}

		[Fact]
		public async Task ChangeUser_Fail_StatusCode400()
		{
			var user = new UserViewModel();
			A.CallTo(() => userChanger.ChangeAsync(A<ClaimsPrincipal>.Ignored, A<UserViewModel>.Ignored)).Returns(ValueResult<UserViewModel>.FromFail());

			var actionResult = await userController.ChangeUser(user);

			Assert.IsType<BadRequestObjectResult>(actionResult.Result);
		}

		[Fact]
		public async Task ChangeUser_Success_StatusCode200()
		{
			var user = new UserViewModel();
			A.CallTo(() => userChanger.ChangeAsync(A<ClaimsPrincipal>.Ignored, A<UserViewModel>.Ignored)).Returns(ValueResult<UserViewModel>.FromValue(user));

			var actionResult = await userController.ChangeUser(user);

			Assert.IsType<OkObjectResult>(actionResult.Result);
		}

		[Fact]
		public async Task ChangePassword_Fail_StatusCode400()
		{
			A.CallTo(() => userChanger.ChangePasswordAsync(A<ClaimsPrincipal>.Ignored, A<string>.Ignored)).Returns(Result.FromFail());

			JsonPatchDocument<UserPassword> patch = new();
			var actionResult = await userController.ChangePassword(patch);

			Assert.IsType<BadRequestObjectResult>(actionResult);
		}

		[Fact]
		public async Task ChangePassword_Success_StatusCode200()
		{
			A.CallTo(() => userChanger.ChangePasswordAsync(A<ClaimsPrincipal>.Ignored, A<string>.Ignored)).Returns(Result.Success);

			JsonPatchDocument<UserPassword> patch = new();
			var actionResult = await userController.ChangePassword(patch);

			Assert.IsType<OkResult>(actionResult);
		}
	}
}
