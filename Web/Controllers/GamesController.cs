using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Data.Constants;
using WebAppLab.Services;
using WebAppLab.Models;
using WebAppLab.Constants;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GamesController : ControllerBase
	{
		private readonly IProductManager productManager;

		public GamesController(IProductManager productManager)
		{
			this.productManager = productManager;
		}

		[HttpGet("topPlatforms")]
		public async Task<List<Platforms>> GetTopPlatforms() => await productManager.GetTopAsync(3);

		[HttpGet("search")]
		public async Task<List<ProductViewModel>> SearchProducts(string term, int limit, int offset) => await productManager.FindByNameAsync(term, limit, offset);

		[HttpGet("id/{id}")]
		public async Task<ActionResult<ProductViewModel>> GetProduct(Guid id)
		{
			var product = await productManager.FindByIdAsync(id);
			if (product == null)
				return NotFound();

			return product;
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPost]
		public async Task<ActionResult<ProductViewModel>> CreateProduct([FromForm] ProductForm product)
		{
			var productView = await productManager.CreateAsync(product);
			return StatusCode(201, productView);
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPut]
		public async Task<ActionResult<ProductViewModel>> UpdateProduct([FromForm] ProductForm product) =>
			await productManager.UpdateAsync(product);

		[Authorize(Roles = Roles.Admin)]
		[HttpDelete("id/{id}")]
		public async Task<IActionResult> DeleteProduct(Guid id)
		{
			await productManager.DeleteAsync(id);
			return NoContent();
		}
	}
}
