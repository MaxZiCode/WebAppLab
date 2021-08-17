using FakeItEasy;
using AutoFixture;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Infrastructure;
using WebAppLab.Services;
using WebAppLab.Wrappers;

using Xunit;
using WebAppLab.Models;
using System.Collections.Generic;
using WebAppLab.Data;

namespace WebAppLab.Tests
{
	public class OrderManagerTests : DbRequiredTests
	{
		[Fact]
		public async Task Can_add_product_and_create_new_order_if_not_exists()
		{
			GetAddProductArgs(out string userId, out AddProductData addProductData);

			await TestOrderManagerMethod(om => om.AddProductAsync(userId, addProductData));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			Assert.Single(assertContext.Orders.AsNoTracking().Where(o => o.UserId == userId));
		}

		[Fact]
		public async Task Can_add_product_and_create_new_productAmount_if_not_exists_in_order()
		{
			GetAddProductArgs(out string userId, out AddProductData addProductData);
			using var arrangeContext = new ApplicationDbContext(ContextOptions);
			arrangeContext.Orders.Add(new Order() { UserId = userId });
			await arrangeContext.SaveChangesAsync();

			await TestOrderManagerMethod(om => om.AddProductAsync(userId, addProductData));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			Assert.Single(assertContext.ProductAmounts.AsNoTracking().Where(pa => pa.ProductId == addProductData.ProductId && pa.Amount == addProductData.Amount));
		}

		[Fact]
		public async Task Can_add_product_and_increace_ammount_of_productAmount_if_exists()
		{
			GetAddProductArgs(out string userId, out AddProductData addProductData);
			using var arrangeContext = new ApplicationDbContext(ContextOptions);
			Order arrangeOrder = new()
			{
				UserId = userId,
				ProductAmounts = new HashSet<ProductAmount>(new[] { new ProductAmount()
				{
					ProductId = addProductData.ProductId,
					Amount = addProductData.Amount
				} })
			};
			arrangeContext.Orders.Add(arrangeOrder);
			await arrangeContext.SaveChangesAsync();

			await TestOrderManagerMethod(om => om.AddProductAsync(userId, addProductData));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var assertProductAmount = await assertContext.ProductAmounts.AsNoTracking().SingleAsync(pa => pa.ProductId == addProductData.ProductId);
			int expectedAmount = addProductData.Amount * 2;
			int actualAmount = assertProductAmount.Amount;
			Assert.Equal(expectedAmount, actualAmount);
		}

		private void GetAddProductArgs(out string userId, out AddProductData addProductData)
		{
			userId = Guid.NewGuid().ToString();
			addProductData = new()
			{
				Amount = new Random().Next(0, 100),
				ProductId = Guid.NewGuid()
			};

		}

		private async Task TestOrderManagerMethod(Func<OrderManager, Task> methodPicker)
		{
			using var context = new ApplicationDbContext(ContextOptions);
			OrderManager orderManager = new(context);
			await methodPicker(orderManager);
		}
	}
}
