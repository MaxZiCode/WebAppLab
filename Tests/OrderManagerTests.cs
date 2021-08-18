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
		public OrderManagerTests() : base()
		{
			Seed();
		}

		private void Seed()
		{
			Random random = new();
			using var context = new ApplicationDbContext(ContextOptions);
			for (int i = 0; i < 5; i++)
			{
				Order order = new()
				{
					Id = Guid.NewGuid(),
					UserId = Guid.NewGuid().ToString(),
					ProductAmounts = new HashSet<ProductAmount>()
				};
				for (int y = 0; y < i; y++)
				{
					order.ProductAmounts.Add(new()
					{
						OrderId = order.Id,
						ProductId = Guid.NewGuid(),
						Amount = random.Next(1, 50),
					});
				}
				context.Orders.Add(order);
			}
			context.SaveChanges();
		}

		[Fact]
		public async Task Can_add_product_and_create_new_order_if_not_exists()
		{
			GetAddProductArgs(out string userId, out AddProductData addProductData);

			await TestOrderManagerMethod(om => om.AddProductAsync(userId, addProductData));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			Assert.Single(assertContext.Orders.Where(o => o.UserId == userId));
		}

		[Fact]
		public async Task Can_add_product_and_create_new_productAmount_if_not_exists_in_order()
		{
			GetAddProductArgs(out string userId, out AddProductData addProductData);
			using var arrangeContext = new ApplicationDbContext(ContextOptions);

			Order arrangeOrder = await GetFirstOrderAsync();
			arrangeOrder.UserId = userId;

			await arrangeContext.SaveChangesAsync();

			await TestOrderManagerMethod(om => om.AddProductAsync(userId, addProductData));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			Assert.Single(assertContext.ProductAmounts.Where(pa => pa.ProductId == addProductData.ProductId && pa.Amount == addProductData.Amount));
		}

		[Fact]
		public async Task Can_add_product_and_increace_ammount_of_productAmount_if_exists()
		{
			GetAddProductArgs(out string userId, out AddProductData addProductData);
			using var arrangeContext = new ApplicationDbContext(ContextOptions);

			Order arrangeOrder = arrangeContext.Orders.Include(o => o.ProductAmounts).Where(o => o.ProductAmounts.Count > 0).First();
			arrangeOrder.UserId = userId;

			ProductAmount arrangeProductAmount = arrangeOrder.ProductAmounts.First();
			addProductData.ProductId = arrangeProductAmount.ProductId;
			int arrangeAmount = arrangeProductAmount.Amount;

			await arrangeContext.SaveChangesAsync();

			addProductData.ProductId = arrangeProductAmount.ProductId;
			await TestOrderManagerMethod(om => om.AddProductAsync(userId, addProductData));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var assertProductAmount = await assertContext.ProductAmounts.SingleAsync(pa => pa.ProductId == addProductData.ProductId);
			int expectedAmount = arrangeAmount + addProductData.Amount;
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

		[Fact]
		public async Task Can_get_products_with_userId_and_orderId_choose_orderId()
		{
			using var context = new ApplicationDbContext(ContextOptions);
			var twoOrders = await context.Orders.Include(o => o.ProductAmounts).Where(o => o.ProductAmounts.Count > 1).Take(2).ToListAsync();
			Guid orderId = twoOrders[0].Id;
			string userId = twoOrders[1].UserId;

			var products = await TestOrderManagerMethod(om => om.GetProductsAsync(userId, orderId));

			Assert.All(products, pa => Assert.Equal(orderId, pa.OrderId));
		}

		[Fact]
		public async Task Can_get_products_with_userId()
		{
			Order order = await GetFirstOrderAsync();

			var products = await TestOrderManagerMethod(om => om.GetProductsAsync(order.UserId));

			Assert.All(products, pa => Assert.Equal(order.Id, pa.OrderId));
		}

		[Fact]
		public async Task Can_get_products_with_orderId()
		{
			Order order = await GetFirstOrderAsync();

			var products = await TestOrderManagerMethod(om => om.GetProductsAsync(null, order.Id));

			Assert.All(products, pa => Assert.Equal(order.Id, pa.OrderId));
		}

		[Fact]
		public async Task Cannot_get_products_with_null_arguments()
		{
			var products = await TestOrderManagerMethod(om => om.GetProductsAsync(null, null));

			Assert.Empty(products);
		}

		[Fact]
		public async Task Can_delete_products()
		{
			var arrangeOrder = await GetFirstOrderAsync();
			var twoProductIds = arrangeOrder.ProductAmounts.Take(2).Select(pa => pa.ProductId).ToList();

			await TestOrderManagerMethod(om => om.DeleteProductsAsync(arrangeOrder.UserId, twoProductIds));

			var assertOrder = await GetFirstOrderAsync();
			Assert.DoesNotContain(assertOrder.ProductAmounts, pa => twoProductIds.Contains(pa.ProductId));
		}

		[Fact]
		public async Task Cannot_delete_products_if_order_for_user_not_found()
		{
			var arrangeOrder = await GetFirstOrderAsync();
			var productIds = arrangeOrder.ProductAmounts.Select(pa => pa.ProductId).ToList();
			var userId = Guid.NewGuid().ToString();

			await TestOrderManagerMethod(om => om.DeleteProductsAsync(userId, productIds));

			var assertOrder = await GetFirstOrderAsync();
			Assert.Contains(assertOrder.ProductAmounts, pa => productIds.Contains(pa.ProductId));
		}

		[Fact]
		public async Task Can_delete_products_and_order_if_no_products()
		{
			var arrangeOrder = await GetFirstOrderAsync();
			var productIds = arrangeOrder.ProductAmounts.Select(pa => pa.ProductId).ToList();

			await TestOrderManagerMethod(om => om.DeleteProductsAsync(arrangeOrder.UserId, productIds));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var assertOrderIds = await assertContext.Orders.Select(o => o.Id).ToListAsync();
			Assert.DoesNotContain(arrangeOrder.Id, assertOrderIds);
		}

		[Fact]
		public async Task Can_buy_products()
		{
			var arrangeOrder = await GetFirstOrderAsync();

			await TestOrderManagerMethod(om => om.BuyProductsAsync(arrangeOrder.UserId));

			var assertOrder = await GetFirstOrderAsync();
			Assert.True(assertOrder.IsCompleted);
		}

		[Fact]
		public async Task Cannot_buy_products_for_user_without_order()
		{
			var userId = Guid.NewGuid().ToString();

			await TestOrderManagerMethod(om => om.BuyProductsAsync(userId));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var assertOrders = await assertContext.Orders.ToListAsync();
			Assert.DoesNotContain(assertOrders, o => o.IsCompleted);
		}

		public async Task<Order> GetFirstOrderAsync()
		{
			using var context = new ApplicationDbContext(ContextOptions);
			return await context.Orders.Include(o => o.ProductAmounts).Where(o => o.ProductAmounts.Count > 1).FirstAsync();
		}

		private async Task TestOrderManagerMethod(Func<OrderManager, Task> methodPicker)
		{
			using var context = new ApplicationDbContext(ContextOptions);
			OrderManager orderManager = new(context);
			await methodPicker(orderManager);
		}

		private async Task<T> TestOrderManagerMethod<T>(Func<OrderManager, Task<T>> methodPicker)
		{
			using var context = new ApplicationDbContext(ContextOptions);
			OrderManager orderManager = new(context);
			return await methodPicker(orderManager);
		}
	}
}
