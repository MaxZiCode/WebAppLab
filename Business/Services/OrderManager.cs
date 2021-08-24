using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Models;

namespace WebAppLab.Services
{
	public class OrderManager : IOrderManager
	{
		private readonly IOrderRepository orderRepository;

		public OrderManager(IOrderRepository orderRepository)
		{
			this.orderRepository = orderRepository;
		}

		public async Task<Order> AddProductAsync(string userId, AddProductData data)
		{
			Order order = await orderRepository.GetForUserAsync(userId, track: true, includeProductAmounts: true);
			if (order == null)
			{
				order = new() { UserId = userId, ProductAmounts = new HashSet<ProductAmount>() };
				await orderRepository.AddAsync(order);
			}

			ProductAmount productAmount = order.ProductAmounts.SingleOrDefault(pa => pa.ProductId == data.ProductId);
			if (productAmount == null)
			{
				productAmount = new() { ProductId = data.ProductId, OrderId = order.Id, Amount = data.Amount };
				order.ProductAmounts.Add(productAmount);
			}
			else
			{
				productAmount.Amount += data.Amount;
			}

			await orderRepository.SaveAsync();
			return order;
		}

		public async Task<IEnumerable<ProductAmount>> GetProductsAsync(string userId, Guid? orderId = null)
		{
			Order order =
				orderId.HasValue ?
				await orderRepository.GetAsync(orderId.Value, includeProductAmounts: true) :
				await orderRepository.GetForUserAsync(userId, includeProductAmounts: true);
			if (order == null)
				return Enumerable.Empty<ProductAmount>();

			return order.ProductAmounts;
		}

		public async Task DeleteProductsAsync(string userId, IEnumerable<Guid> productIds)
		{
			var order = await orderRepository.GetForUserAsync(userId, includeProductAmounts: true, track: true);
			if (order == null)
				return;

			var removeItems = order.ProductAmounts.Where(pa => productIds.Contains(pa.ProductId)).ToList();
			foreach (var removeItem in removeItems)
				order.ProductAmounts.Remove(removeItem);

			if (!order.ProductAmounts.Any())
				orderRepository.Remove(order);

			await orderRepository.SaveAsync();
		}

		public async Task BuyProductsAsync(string userId)
		{
			var order = await orderRepository.GetForUserAsync(userId, track: true);
			if (order == null)
				return;

			order.IsCompleted = true;

			await orderRepository.SaveAsync();
		}
	}
}
