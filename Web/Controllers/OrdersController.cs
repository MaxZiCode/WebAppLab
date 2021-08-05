using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Models;
using WebAppLab.Extensions;
using System.Linq.Expressions;

namespace WebAppLab.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public OrdersController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpPost]
		public async Task<Order> AddProduct([FromBody] AddProductData data)
		{
			var userId = HttpContext.User.GetId();

			Order order = await context.Orders.Include(o => o.ProductAmounts).FirstOrDefaultAsync(o => o.UserId == userId);
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

		[HttpGet]
		public async Task<IEnumerable<ProductAmount>> GetProducts(Guid? orderId = null)
		{
			Guid? orderToSearch = orderId ?? await GetOrderIdOfCurrentUser();
			if (!orderToSearch.HasValue)
				return null;

			var productAmountList = await context.ProductAmounts.Where(pa => pa.OrderId == orderToSearch).ToListAsync();
			if (productAmountList.Count == 0)
				return null;

			return productAmountList;
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteProducts([FromBody] IEnumerable<Guid> productIds)
		{
			var orderId = await GetOrderIdOfCurrentUser();
			foreach (var productId in productIds)
			{
				var productAmount = await context.ProductAmounts.FindAsync(orderId, productId);
				if (productAmount != null)
					context.ProductAmounts.Remove(productAmount);
			}

			await context.SaveChangesAsync();
			return NoContent();
		}

		private async Task<Guid?> GetOrderIdOfCurrentUser()
		{
			var userId = HttpContext.User.GetId();
			var order = await context.Orders
				.FirstOrDefaultAsync(o => o.UserId == userId);
			return order?.Id;
		}
	}
}
