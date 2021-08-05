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
using WebAppLab.Services;

namespace WebAppLab.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly IOrderManager orderManager;

		public OrdersController(IOrderManager orderManager)
		{
			this.orderManager = orderManager;
		}

		[HttpPost]
		public async Task<IActionResult> AddProduct([FromBody] AddProductData data)
		{
			Order order = await orderManager.AddProductAsync(HttpContext.User, data);
			return StatusCode(201, order);
		}

		[HttpGet]
		public async Task<IActionResult> GetProducts(Guid? orderId = null)
		{
			var hasUserOrder = await orderManager.HasUserOrderAsync(HttpContext.User);
			if (!hasUserOrder)
				return NotFound();

			var products = await orderManager.GetProductsAsync(HttpContext.User, orderId);
			if (!products.Any())
				return NoContent();

			return Ok(products);
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteProducts([FromBody] IEnumerable<Guid> productIds)
		{
			var hasUserOrder = await orderManager.HasUserOrderAsync(HttpContext.User);
			if (!hasUserOrder)
				return NotFound();

			await orderManager.DeleteProductsAsync(HttpContext.User, productIds);
			return NoContent();
		}

		[HttpPost("buy")]
		public async Task<IActionResult> BuyProducts()
		{
			var hasUserOrder = await orderManager.HasUserOrderAsync(HttpContext.User);
			if (!hasUserOrder)
				return NotFound();

			await orderManager.BuyProductsAsync(HttpContext.User);
			return NoContent();
		}
	}
}
