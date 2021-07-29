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
		public async Task<List<Platforms>> GetTopPlatforms() => await productManager.GetTop(3);

		[HttpGet("search")]
		public async Task<List<ProductViewModel>> SearchProducts(string term, int limit, int offset) => await productManager.FindByName(term, limit, offset);

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
		public async Task<ActionResult<ProductViewModel>> CreateProduct([FromForm] string product, [FromForm] IFormFile logo, [FromForm] IFormFile background)
		{
			var productView = await productManager.Create(product, logo, background);
			return StatusCode(201, productView);
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPut]
		public async Task<ActionResult<ProductViewModel>> UpdateProduct([FromForm] string product, [FromForm] IFormFile logo, [FromForm] IFormFile background) =>
			await productManager.Update(product, logo, background);

		[Authorize(Roles = Roles.Admin)]
		[HttpDelete("id/{id}")]
		public async Task<IActionResult> DeleteProduct(Guid id)
		{
			await productManager.Delete(id);
			return NoContent();
		}
	}
}
