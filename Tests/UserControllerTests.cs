using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

using WebAppLab.Controllers;
using WebAppLab.Models;
using WebAppLab.Services;
using WebAppLab.Wrappers;

using Xunit;

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
		public async Task GetUser_UserExists_OkObjectResult()
		{
			A.CallTo(() => userChanger.GetUserAsync(A<string>.Ignored)).Returns(A.Fake<UserViewModel>());

			var actionResult = await userController.GetUser();

			Assert.IsType<OkObjectResult>(actionResult.Result);
		}

		[Fact]
		public async Task ChangeUser_InvalidUserViewModel_BadRequestObjectResult()
		{
			var user = new UserViewModel();
			A.CallTo(() => userChanger.ChangeAsync(A<string>.Ignored, A<UserViewModel>.Ignored)).Returns(ValueResult<UserViewModel>.FromFail());

			var actionResult = await userController.ChangeUser(user);

			Assert.IsType<BadRequestObjectResult>(actionResult.Result);
		}

		[Fact]
		public async Task ChangeUser_ValidUserViewModel_OkObjectResult()
		{
			var user = new UserViewModel();
			A.CallTo(() => userChanger.ChangeAsync(A<string>.Ignored, A<UserViewModel>.Ignored)).Returns(ValueResult<UserViewModel>.FromValue(user));

			var actionResult = await userController.ChangeUser(user);

			Assert.IsType<OkObjectResult>(actionResult.Result);
		}

		[Fact]
		public async Task ChangePassword_PatchWithInvalidPassword_BadRequestObjectResult()
		{
			A.CallTo(() => userChanger.ChangePasswordAsync(A<string>.Ignored, A<string>.Ignored)).Returns(Result.FromFail());

			JsonPatchDocument<UserPassword> patch = new();
			var actionResult = await userController.ChangePassword(patch);

			Assert.IsType<BadRequestObjectResult>(actionResult);
		}

		[Fact]
		public async Task ChangePassword_PatchWithValidPassword_OkResult()
		{
			A.CallTo(() => userChanger.ChangePasswordAsync(A<string>.Ignored, A<string>.Ignored)).Returns(Result.Success);

			JsonPatchDocument<UserPassword> patch = new();
			var actionResult = await userController.ChangePassword(patch);

			Assert.IsType<OkResult>(actionResult);
		}
	}
}
