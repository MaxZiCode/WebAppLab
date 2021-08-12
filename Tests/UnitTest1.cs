using System;

using Xunit;
using FakeItEasy;
using WebAppLab.Services;

namespace WebAppLab.Tests
{
	public class UserControllerTests
	{
		[Fact]
		public void Test1()
		{
			var fakeUser = A.Fake<IUserChanger>();


		}
	}
}
