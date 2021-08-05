using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Extensions;
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

		public async Task<Order> AddProductAsync(ClaimsPrincipal user, AddProductData data)
		{
			var userId = user.GetId();
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

		public async Task<IEnumerable<ProductAmount>> GetProductsAsync(ClaimsPrincipal user, Guid? orderId = null)
		{
			Guid? orderToSearch = orderId ?? (await GetOrderForUserAsync(user))?.Id;
			if (!orderToSearch.HasValue)
				return null;

			var productAmountList = await context.ProductAmounts.Where(pa => pa.OrderId == orderToSearch).ToListAsync();
			return productAmountList;
		}

		public async Task DeleteProductsAsync(ClaimsPrincipal user, IEnumerable<Guid> productIds)
		{
			var order = await GetOrderForUserAsync(user, includeProductAmounts: true);
			if (order == null)
				return;

			var removeItems = order.ProductAmounts.Where(pa => productIds.Contains(pa.ProductId)).ToList();
			foreach (var removeItem in removeItems)
				order.ProductAmounts.Remove(removeItem);

			if (!order.ProductAmounts.Any())
				context.Orders.Remove(order);

			await context.SaveChangesAsync();
		}

		public async Task BuyProductsAsync(ClaimsPrincipal user)
		{
			var order = await GetOrderForUserAsync(user);
			if (order == null)
				return;

			order.IsCompleted = true;

			await context.SaveChangesAsync();
		}

		public async Task<bool> HasUserOrderAsync(ClaimsPrincipal user) => await GetOrderForUserAsync(user) != null;

		private async Task<Order> GetOrderForUserAsync(ClaimsPrincipal user, bool includeProductAmounts = false)
		{
			var userId = user.GetId();
			return await GetOrderForUserAsync(userId, includeProductAmounts);
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
