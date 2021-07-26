using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Services;
using WebAppLab.Constants;
using Microsoft.Extensions.Logging;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GamesController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IProductManager productManager;

		public GamesController(ApplicationDbContext context, IProductManager productManager)
		{
			this.context = context;
			this.productManager = productManager;
		}

		[HttpGet("topPlatforms")]
		public IEnumerable<string> GetTopPlatforms() => productManager.GetTop(3);

		[HttpGet("search")]
		public IEnumerable<Product> SearchProducts(string term, int limit, int offset) => productManager.FindByName(term, limit, offset);

		[HttpGet("id/{id}")]
		public async Task<ActionResult<Product>> GetProduct(Guid id)
		{
			var product = await FindProductAsync(id);
			if (product == null)
				return NotFound();

			return product;
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPost]
		public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
		{
			await context.Products.AddAsync(product);
			await context.SaveChangesAsync();
			var createdProduct = await FindProductAsync(product.Id);
			return StatusCode(201, createdProduct);
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPut]
		public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product)
		{
			context.Update(product);
			await context.SaveChangesAsync();
			return await FindProductAsync(product.Id);
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpDelete("id/{id}")]
		public async Task<IActionResult> DeleteProduct(Guid id)
		{
			var product = await FindProductAsync(id);
			if (product == null)
				return NotFound();

			context.Products.Remove(product);
			await context.SaveChangesAsync();

			return NoContent();
		}

		private async Task<Product> FindProductAsync(Guid id) => await context.Products.FindAsync(id);
	}
}
