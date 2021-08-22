using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Models;
using WebAppLab.Services;

using Xunit;

namespace WebAppLab.Tests
{
	public class OrderManagerTests : DbRequiredTests
	{
		[Fact]
		public async Task Can_add_product_and_create_new_order()
		{
			GetAddProductArgs(out string userId, out AddProductData addProductData);

			var resultOrder = await TestOrderManagerMethod(om => om.AddProductAsync(userId, addProductData));

			Order dbOrder = await GetOrderForUserAsync(userId);
			Assert.NotNull(dbOrder);
		}

		[Fact]
		public async Task Can_add_product_to_existing_order()
		{
			GetAddProductArgs(out string userId, out AddProductData firstAddProductData);
			GetAddProductArgs(out _, out AddProductData secondAddProductData);

			await TestOrderManagerMethod(om => om.AddProductAsync(userId, firstAddProductData));
			await TestOrderManagerMethod(om => om.AddProductAsync(userId, secondAddProductData));

			Order dbOrder = await GetOrderForUserAsync(userId);
			Assert.Contains(dbOrder.ProductAmounts, pa => pa.ProductId == firstAddProductData.ProductId);
			Assert.Contains(dbOrder.ProductAmounts, pa => pa.ProductId == secondAddProductData.ProductId);
		}

		[Fact]
		public async Task Can_add_product_that_exists_and_increase_amount()
		{
			GetAddProductArgs(out string userId, out AddProductData firstAddProductData);
			GetAddProductArgs(out _, out AddProductData secondAddProductData);
			secondAddProductData.ProductId = firstAddProductData.ProductId;

			await TestOrderManagerMethod(om => om.AddProductAsync(userId, firstAddProductData));
			await TestOrderManagerMethod(om => om.AddProductAsync(userId, secondAddProductData));

			Order dbOrder = await GetOrderForUserAsync(userId);
			int expected = firstAddProductData.Amount + secondAddProductData.Amount;
			int actual = dbOrder.ProductAmounts.First().Amount;
			Assert.Equal(expected, actual);
		}

		private async Task<Order> GetOrderForUserAsync(string userId)
		{
			using var assertContext = new ApplicationDbContext(ContextOptions);
			return await OrdersHelper.GetOrderForUserAsync(assertContext.Orders, userId, includeProductAmounts: true);
		}

		private void GetAddProductArgs(out string userId, out AddProductData addProductData)
		{
			userId = Guid.NewGuid().ToString();
			addProductData = new()
			{
				Amount = new Random().Next(1, 100),
				ProductId = Guid.NewGuid()
			};
		}

		[Fact]
		public async Task Can_get_products_with_userId_and_orderId_choose_orderId()
		{
			var orders = GenerateOrders().ToList();
			var userId = orders[0].UserId;
			var orderId = orders[1].Id;
			await SeedOrders(orders);

			var products = await TestOrderManagerMethod(om => om.GetProductsAsync(userId, orderId));

			Assert.All(products, pa => Assert.Equal(orderId, pa.OrderId));
		}

		[Fact]
		public async Task Can_get_products_with_userId()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			await SeedOrders(orders);

			var products = await TestOrderManagerMethod(om => om.GetProductsAsync(order.UserId));

			Assert.All(products, pa => Assert.Equal(order.Id, pa.OrderId));
		}

		[Fact]
		public async Task Can_get_products_with_orderId()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			await SeedOrders(orders);

			var products = await TestOrderManagerMethod(om => om.GetProductsAsync(null, order.Id));

			Assert.All(products, pa => Assert.Equal(order.Id, pa.OrderId));
		}

		[Fact]
		public async Task Cannot_get_products_with_null_arguments()
		{
			var orders = GenerateOrders();
			await SeedOrders(orders);

			var products = await TestOrderManagerMethod(om => om.GetProductsAsync(null, null));

			Assert.Empty(products);
		}

		[Fact]
		public async Task Can_delete_products()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			var productIdsToDelete = order.ProductAmounts.Take(2).Select(p => p.ProductId).ToList();
			await SeedOrders(orders);

			await TestOrderManagerMethod(om => om.DeleteProductsAsync(order.UserId, productIdsToDelete));

			using var context = new ApplicationDbContext(ContextOptions);
			var assertOrder = await context.Orders.Include(o => o.ProductAmounts).FirstAsync(o => o.Id == order.Id);
			Assert.DoesNotContain(assertOrder.ProductAmounts, pa => productIdsToDelete.Contains(pa.ProductId));
		}

		[Fact]
		public async Task Cannot_delete_products_if_order_for_user_not_found()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			string userId = Guid.NewGuid().ToString();
			var productIdsToDelete = order.ProductAmounts.Select(p => p.ProductId).ToList();
			await SeedOrders(orders);

			await TestOrderManagerMethod(om => om.DeleteProductsAsync(userId, productIdsToDelete));

			var expected = orders.SelectMany(o => o.ProductAmounts).Count();
			using var context = new ApplicationDbContext(ContextOptions);
			var actual = await context.Orders.Include(o => o.ProductAmounts).SelectMany(o => o.ProductAmounts).CountAsync();
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Can_delete_products_and_order_when_no_products()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			var productIdsToDelete = order.ProductAmounts.Select(p => p.ProductId).ToList();
			await SeedOrders(orders);

			await TestOrderManagerMethod(om => om.DeleteProductsAsync(order.UserId, productIdsToDelete));

			using var context = new ApplicationDbContext(ContextOptions);
			var assertOrder = await context.Orders.FindAsync(order.Id);
			Assert.Null(assertOrder);
		}

		[Fact]
		public async Task Can_buy_products()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			await SeedOrders(orders);

			await TestOrderManagerMethod(om => om.BuyProductsAsync(order.UserId));

			using var context = new ApplicationDbContext(ContextOptions);
			var assertOrder = await context.Orders.FindAsync(order.Id);
			Assert.True(assertOrder.IsCompleted);
		}

		[Fact]
		public async Task Cannot_buy_products_for_user_without_order()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			string userId = Guid.NewGuid().ToString();
			await SeedOrders(orders);

			await TestOrderManagerMethod(om => om.BuyProductsAsync(userId));

			using var context = new ApplicationDbContext(ContextOptions);
			var assertOrder = await context.Orders.FindAsync(order.Id);
			Assert.False(assertOrder.IsCompleted);
		}

		private IEnumerable<Order> GenerateOrders(int count = 3)
		{
			Random random = new();
			for (int i = 0; i < count; i++)
			{
				var orderId = Guid.NewGuid();
				Order order = new()
				{
					Id = orderId,
					UserId = Guid.NewGuid().ToString(),
					ProductAmounts = new HashSet<ProductAmount>()
				};
				for (int y = 0; y < 3; y++)
				{
					order.ProductAmounts.Add(new()
					{
						OrderId = orderId,
						ProductId = Guid.NewGuid(),
						Amount = random.Next(1, 50),
					});
				}
				yield return order;
			}
		}

		private async Task SeedOrders(IEnumerable<Order> orders)
		{
			using var context = new ApplicationDbContext(ContextOptions);
			await context.AddRangeAsync(orders);
			await context.SaveChangesAsync();
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
