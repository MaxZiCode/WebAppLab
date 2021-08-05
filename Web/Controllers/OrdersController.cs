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
		public async Task<Order> AddProductToOrder([FromBody] AddProductData data)
		{
			var userId = HttpContext.User.GetId();

			Order order = await context.Orders.Include(o => o.ProductAmounts).FirstOrDefaultAsync(o => o.UserId == userId);
			if (order == null)
			{
				order = new() { UserId = userId, ProductAmounts = new HashSet<ProductAmount>()};
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
			Expression<Func<Order, bool>> searchOption = 
				orderId == null ? 
				(o => o.UserId == HttpContext.User.GetId()) : 
				(o => o.Id == orderId);

			var order = await context.Orders.Include(o => o.ProductAmounts).FirstOrDefaultAsync(searchOption);

			return order.ProductAmounts;
		}
	}
}
