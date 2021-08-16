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

namespace WebAppLab.Tests
{
	public class UserControllerTests
	{
		private readonly IUserChanger userChanger;
		private readonly UserController userController;

		public UserControllerTests()
		{
			userChanger = A.Fake<IUserChanger>();
			var fixture = new Fixture();
			fixture.Register(() => userChanger);
			fixture.Register(() => A.Fake<MethodInfo>());
			fixture.Register(() => A.Fake<TypeInfo>());
			fixture.Register(() => A.Fake<Microsoft.AspNetCore.Mvc.ActionConstraints.IActionConstraintMetadata>());
			fixture.Register(() => A.Fake<Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo>());
			userController = fixture.Create<UserController>();
		}

		[Fact]
		public async Task GetUser_calls_GetUserAsync_from_UserChanger()
		{
			await userController.GetUser();

			A.CallTo(() => userChanger.GetUserAsync(A<ClaimsPrincipal>.Ignored)).MustHaveHappened();
		}
	}
}
