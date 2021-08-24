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
	public class OrderManagerTests : DbRequiredTests, IAsyncDisposable
	{
		private readonly ApplicationDbContext context;
		private readonly OrderManager orderManager;

		public OrderManagerTests()
		{
			context = new ApplicationDbContext(ContextOptions);
			orderManager = new OrderManager(context);
		}

		[Fact]
		public async Task AddProductAsync_ValidData_OrderExistsInDb()
		{
			GetAddProductArgs(out string userId, out AddProductData addProductData);

			await orderManager.AddProductAsync(userId, addProductData);

			Order dbOrder = await OrdersHelper.GetOrderForUserAsync(context.Orders, userId);
			Assert.NotNull(dbOrder);
		}

		[Fact]
		public async Task AddProductAsync_TwoAddProductDatasSameUser_OneOrderContainsAllProducts()
		{
			GetAddProductArgs(out string userId, out AddProductData firstAddProductData);
			GetAddProductArgs(out _, out AddProductData secondAddProductData);

			await orderManager.AddProductAsync(userId, firstAddProductData);
			await orderManager.AddProductAsync(userId, secondAddProductData);

			Order dbOrder = await OrdersHelper.GetOrderForUserAsync(context.Orders, userId, includeProductAmounts: true);
			Assert.Contains(dbOrder.ProductAmounts, pa => pa.ProductId == firstAddProductData.ProductId);
			Assert.Contains(dbOrder.ProductAmounts, pa => pa.ProductId == secondAddProductData.ProductId);
		}

		[Fact]
		public async Task AddProductAsync_TwoAddProductDatasSameUserSameProductId_ProductAmountContainsSumOfAllAmounts()
		{
			GetAddProductArgs(out string userId, out AddProductData firstAddProductData);
			GetAddProductArgs(out _, out AddProductData secondAddProductData);
			secondAddProductData.ProductId = firstAddProductData.ProductId;

			await orderManager.AddProductAsync(userId, firstAddProductData);
			await orderManager.AddProductAsync(userId, secondAddProductData);

			Order dbOrder = await OrdersHelper.GetOrderForUserAsync(context.Orders, userId, includeProductAmounts: true);
			int expected = firstAddProductData.Amount + secondAddProductData.Amount;
			int actual = dbOrder.ProductAmounts.First().Amount;
			Assert.Equal(expected, actual);
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
		public async Task GetProductsAsync_ExistingUserIdOrderId_ReturnsAllProductAmountsFromOrderWithThisId()
		{
			var orders = GenerateOrders().ToList();
			var userId = orders[0].UserId;
			var orderId = orders[1].Id;
			await SeedOrders(orders);

			var productAmounts = await orderManager.GetProductsAsync(userId, orderId);

			Assert.All(productAmounts, pa => Assert.Equal(orderId, pa.OrderId));
		}

		[Fact]
		public async Task GetProductsAsync_ExistingUserId_ReturnsAllProductAmountsFromOrderWithUserId()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			await SeedOrders(orders);

			var products = await orderManager.GetProductsAsync(order.UserId);

			Assert.All(products, pa => Assert.Equal(order.Id, pa.OrderId));
		}

		[Fact]
		public async Task GetProductsAsync_ExistingOrderId_ReturnsAllProductAmountsFromOrderWithThisId()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			await SeedOrders(orders);

			var products = await orderManager.GetProductsAsync(null, order.Id);

			Assert.All(products, pa => Assert.Equal(order.Id, pa.OrderId));
		}

		[Fact]
		public async Task GetProductsAsync_Nulls_ReturnsEmptyProductAmountsCollection()
		{
			var orders = GenerateOrders();
			await SeedOrders(orders);

			var productAmounts = await orderManager.GetProductsAsync(null, null);

			Assert.Empty(productAmounts);
		}

		[Fact]
		public async Task DeleteProductsAsync_SomeProductAmountIdsFromOrder_OrderDoesNotContainsTheseProductAmounts()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			var productIdsToDelete = order.ProductAmounts.Take(2).Select(p => p.ProductId).ToList();
			await SeedOrders(orders);

			await orderManager.DeleteProductsAsync(order.UserId, productIdsToDelete);

			var assertOrder = await context.Orders.Include(o => o.ProductAmounts).FirstAsync(o => o.Id == order.Id);
			Assert.DoesNotContain(assertOrder.ProductAmounts, pa => productIdsToDelete.Contains(pa.ProductId));
		}

		[Fact]
		public async Task DeleteProductsAsync_IdOfUserWithoutOrder_ContextNotChanged()
		{
			var orders = GenerateOrders().ToList();
			await SeedOrders(orders);
			bool changed = false;
			context.ChangeTracker.StateChanged += (o, e) => changed = true;
			string userId = Guid.NewGuid().ToString();
			var productIdsToDelete = orders[0].ProductAmounts.Select(p => p.ProductId).ToList();

			await orderManager.DeleteProductsAsync(userId, productIdsToDelete);

			Assert.False(changed);
		}

		[Fact]
		public async Task DeleteProductsAsync_AllProductAmountIdsFromOrder_ThisOrderHasDeleted()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			var productIdsToDelete = order.ProductAmounts.Select(p => p.ProductId).ToList();
			await SeedOrders(orders);

			await orderManager.DeleteProductsAsync(order.UserId, productIdsToDelete);

			var assertOrder = await context.Orders.FindAsync(order.Id);
			Assert.Null(assertOrder);
		}

		[Fact]
		public async Task BuyProductsAsync_ExistingUserId_OrderOfUserIsCompleted()
		{
			var orders = GenerateOrders().ToList();
			Order order = orders[0];
			await SeedOrders(orders);

			await orderManager.BuyProductsAsync(order.UserId);

			var assertOrder = await context.Orders.FindAsync(order.Id);
			Assert.True(assertOrder.IsCompleted);
		}

		[Fact]
		public async Task BuyProductsAsync_UserWithNoOrderId_ContextNotChanged()
		{
			var orders = GenerateOrders().ToList();
			await SeedOrders(orders);
			string userId = Guid.NewGuid().ToString();
			bool changed = false;
			context.ChangeTracker.StateChanged += (o, e) => changed = true;

			await orderManager.BuyProductsAsync(userId);

			Assert.False(changed);
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
			await context.AddRangeAsync(orders);
			await context.SaveChangesAsync();
		}

		public ValueTask DisposeAsync()
		{
			return ((IAsyncDisposable)context).DisposeAsync();
		}
	}
}
