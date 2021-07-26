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
using WebAppLab.Models;
using WebAppLab.Services;
using WebAppLab.Constants;
using Microsoft.Extensions.Logging;
using AutoMapper;
using WebAppLab.Data.Models;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GamesController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IProductManager productManager;
		private readonly IMapper mapper;

		public GamesController(ApplicationDbContext context, IProductManager productManager, IMapper mapper)
		{
			this.context = context;
			this.productManager = productManager;
			this.mapper = mapper;
		}

		[HttpGet("topPlatforms")]
		public IEnumerable<string> GetTopPlatforms() => productManager.GetTop(3);

		[HttpGet("search")]
		public IEnumerable<ProductViewModel> SearchProducts(string term, int limit, int offset) => productManager.FindByName(term, limit, offset);

		[HttpGet("id/{id}")]
		public async Task<ActionResult<ProductViewModel>> GetProduct(Guid id)
		{
			var product = await FindProductViewAsync(id);
			if (product == null)
				return NotFound();

			return product;
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPost]
		public async Task<ActionResult<ProductViewModel>> CreateProduct([FromBody] ProductViewModel productView)
		{
			var product = mapper.Map<Product>(productView);
			await context.Products.AddAsync(product);
			await context.SaveChangesAsync();
			var createdProduct = await FindProductViewAsync(product.Id);
			return StatusCode(201, createdProduct);
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPut]
		public async Task<ActionResult<ProductViewModel>> UpdateProduct([FromBody] ProductViewModel productView)
		{
			var product = mapper.Map<Product>(productView);
			context.Update(product);
			await context.SaveChangesAsync();
			return await FindProductViewAsync(product.Id);
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpDelete("id/{id}")]
		public async Task<IActionResult> DeleteProduct(Guid id)
		{
			var product = await FindProductAsync(id);
			if (product == null)
				return NotFound();

			product.DeletedAt = DateTime.Now;
			await context.SaveChangesAsync();

			return NoContent();
		}

		private async Task<ProductViewModel> FindProductViewAsync(Guid id)
		{
			var product = await FindProductAsync(id);
			return mapper.Map<ProductViewModel>(product);
		}

		private async Task<Product> FindProductAsync(Guid id)
		{
			return await context.Products.FindAsync(id);
		}
	}
}
