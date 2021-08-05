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
		public async Task<IActionResult> AddProduct([FromBody] AddProductData data)
		{
			Order order = await GetOrderOfCurrentUser(includeProductAmounts: true);
			if (order == null)
			{
				var userId = HttpContext.User.GetId();
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
			return StatusCode(201, order);
		}

		[HttpGet]
		public async Task<IActionResult> GetProducts(Guid? orderId = null)
		{
			Guid? orderToSearch = orderId ?? (await GetOrderOfCurrentUser())?.Id;
			if (!orderToSearch.HasValue)
				return NotFound();

			var productAmountList = await context.ProductAmounts.Where(pa => pa.OrderId == orderToSearch).ToListAsync();
			if (productAmountList.Count == 0)
				return NoContent();

			return Ok(productAmountList);
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteProducts([FromBody] IEnumerable<Guid> productIds)
		{
			var order = await GetOrderOfCurrentUser();
			if (order == null)
				return NotFound();

			foreach (var productId in productIds)
			{
				var productAmount = await context.ProductAmounts.FindAsync(order.Id, productId);
				if (productAmount != null)
					context.ProductAmounts.Remove(productAmount);
			}

			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpPost("buy")]
		public async Task<IActionResult> BuyProducts()
		{
			var order = await GetOrderOfCurrentUser();

			if (order == null)
				return NotFound();

			order.IsCompleted = true;

			await context.SaveChangesAsync();
			return NoContent();
		}

		private async Task<Order> GetOrderOfCurrentUser(bool includeProductAmounts = false)
		{
			var userId = HttpContext.User.GetId();

			IQueryable<Order> orders = context.Orders
				.Where(o => !o.IsCompleted);

			if (includeProductAmounts)
				orders = orders.Include(o => o.ProductAmounts);

			return await orders.FirstOrDefaultAsync(o => o.UserId == userId);
		}
	}
}
