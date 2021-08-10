using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Extensions;
using WebAppLab.Models;
using WebAppLab.Services;

namespace WebAppLab.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly IOrderManager orderManager;

		private string UserId => HttpContext.User.GetId();

		public OrdersController(IOrderManager orderManager)
		{
			this.orderManager = orderManager;
		}

		[HttpPost]
		public async Task<IActionResult> AddProduct([FromBody] AddProductData data)
		{
			Order order = await orderManager.AddProductAsync(UserId, data);
			return StatusCode(201, order);
		}

		[HttpGet]
		public async Task<IActionResult> GetProducts(Guid? orderId = null)
		{
			var products = await orderManager.GetProductsAsync(UserId, orderId);

			return Ok(products);
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteProducts([FromBody] IEnumerable<Guid> productIds)
		{
			await orderManager.DeleteProductsAsync(UserId, productIds);
			return NoContent();
		}

		[HttpPost("buy")]
		public async Task<IActionResult> BuyProducts()
		{
			await orderManager.BuyProductsAsync(UserId);
			return NoContent();
		}
	}
}
