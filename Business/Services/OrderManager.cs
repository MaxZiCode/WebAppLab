using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Models;

namespace WebAppLab.Services
{
	public class OrderManager : IOrderManager
	{
		private readonly ApplicationDbContext context;

		public OrderManager(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<Order> AddProductAsync(string userId, AddProductData data)
		{
			Order order = await GetOrderForUserAsync(userId, includeProductAmounts: true);
			if (order == null)
			{
				order = new() { UserId = userId, ProductAmounts = new HashSet<ProductAmount>() };
				await context.Orders.AddAsync(order);
			}

			ProductAmount productAmount = order.ProductAmounts.FirstOrDefault(pa => pa.ProductId == data.ProductId);
			if (productAmount == null)
			{
				productAmount = new() { ProductId = data.ProductId, OrderId = order.Id, Amount = data.Amount };
				order.ProductAmounts.Add(productAmount);
			}
			else
			{
				productAmount.Amount += data.Amount;
			}

			await context.SaveChangesAsync();
			return order;
		}

		public async Task<IEnumerable<ProductAmount>> GetProductsAsync(string userId, Guid? orderId = null)
		{
			Guid? orderToSearch = orderId ?? (await GetOrderForUserAsync(userId))?.Id;
			if (!orderToSearch.HasValue)
				return Enumerable.Empty<ProductAmount>();

			var productAmountList = await context.ProductAmounts.Where(pa => pa.OrderId == orderToSearch).ToListAsync();
			return productAmountList;
		}

		public async Task DeleteProductsAsync(string userId, IEnumerable<Guid> productIds)
		{
			var order = await GetOrderForUserAsync(userId, includeProductAmounts: true);
			if (order == null)
				return;

			var removeItems = order.ProductAmounts.Where(pa => productIds.Contains(pa.ProductId)).ToList();
			foreach (var removeItem in removeItems)
				order.ProductAmounts.Remove(removeItem);

			if (!order.ProductAmounts.Any())
				context.Orders.Remove(order);

			await context.SaveChangesAsync();
		}

		public async Task BuyProductsAsync(string userId)
		{
			var order = await GetOrderForUserAsync(userId);
			if (order == null)
				return;

			order.IsCompleted = true;

			await context.SaveChangesAsync();
		}

		private async Task<Order> GetOrderForUserAsync(string userId, bool includeProductAmounts = false)
		{
			IQueryable<Order> orders = context.Orders;

			if (includeProductAmounts)
				orders = orders.Include(o => o.ProductAmounts);

			return await orders.FirstOrDefaultAsync(o => o.UserId == userId);
		}
	}
}
