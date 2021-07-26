using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using WebAppLab.Data;
using WebAppLab.Models;
using WebAppLab.Services;
using WebAppLab.Constants;
using Microsoft.Extensions.Logging;
using AutoMapper;
using WebAppLab.Data.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GamesController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IProductManager productManager;
		private readonly IMapper mapper;
		private readonly Cloudinary cloudinary;

		public GamesController(ApplicationDbContext context, IProductManager productManager, IMapper mapper, Cloudinary cloudinary)
		{
			this.context = context;
			this.productManager = productManager;
			this.mapper = mapper;
			this.cloudinary = cloudinary;
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
		public async Task<ActionResult<ProductViewModel>> CreateProduct([FromForm] string product, [FromForm] IFormFile logo, [FromForm] IFormFile background)
		{
			var productView = JsonConvert.DeserializeObject<ProductViewModel>(product);
			var productData = mapper.Map<Product>(productView);
			if (logo != null)
			{
				var result = await UploadImage(logo);
				productData.Logo = result.Url.AbsoluteUri;
			}
			if (background != null)
			{
				var result = await UploadImage(background);
				productData.Background = result.Url.AbsoluteUri;
			}

			await context.Products.AddAsync(productData);
			await context.SaveChangesAsync();
			var createdProduct = await FindProductViewAsync(productData.Id);
			return StatusCode(201, createdProduct);
		}

		[Authorize(Roles = Roles.Admin)]
		[HttpPut]
		public async Task<ActionResult<ProductViewModel>> UpdateProduct([FromForm] string product, [FromForm] IFormFile logo, [FromForm] IFormFile background)
		{
			var productView = JsonConvert.DeserializeObject<ProductViewModel>(product);
			var productData = mapper.Map<Product>(productView);
			if (logo != null)
			{
				var result = await UploadImage(logo);
				productData.Logo = result.Url.AbsoluteUri;
			}
			if (background != null)
			{
				var result = await UploadImage(background);
				productData.Background = result.Url.AbsoluteUri;
			}

			context.Update(productData);
			await context.SaveChangesAsync();
			return await FindProductViewAsync(productData.Id);
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

		private async Task<ImageUploadResult> UploadImage(IFormFile file)
		{
			using var stream = file.OpenReadStream();
			ImageUploadParams imageUploadParams = new()
			{
				File = new(file.FileName, stream),
				Folder = @"WebAppLab/Products"
			};
			return await cloudinary.UploadAsync(imageUploadParams);
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
