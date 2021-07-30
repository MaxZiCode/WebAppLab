using AutoMapper;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Models;
using WebAppLab.Data.Constants;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebAppLab.Services
{
	public class ProductManager : IProductManager
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly IImageUploader imageUploader;

		public ProductManager(ApplicationDbContext context, IMapper mapper, IImageUploader imageUploader)
		{
			this.context = context;
			this.mapper = mapper;
			this.imageUploader = imageUploader;
		}

		public async Task<List<Platforms>> GetTop(int count)
		{
			return await context.Products
				.GroupBy(p => p.Platform)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.Take(count)
				.ToListAsync();
		}

		public async Task<List<ProductViewModel>> FindByName(string name, int maxCount = int.MaxValue, int offset = 0)
		{
			var list = await context.Products
				.Where(p => p.Name.Contains(name))
				.Skip(offset)
				.Take(maxCount)
				.ToListAsync();
			return list.Select(p => mapper.Map<ProductViewModel>(p)).ToList();
		}

		public async Task<ProductViewModel> FindByIdAsync(Guid id)
		{
			var product = await context.Products.FindAsync(id);
			return mapper.Map<ProductViewModel>(product);
		}

		public async Task<ProductViewModel> Create(string productViewJson, IFormFile logo, IFormFile background)
		{
			var product = DeserializeProductAndMap(productViewJson);
			await UploadImagesAndSetToProduct(product, logo, background);

			await context.Products.AddAsync(product);
			await context.SaveChangesAsync();
			return await FindByIdAsync(product.Id);

		}

		public async Task<ProductViewModel> Update(string productViewJson, IFormFile logo, IFormFile background)
		{
			var product = DeserializeProductAndMap(productViewJson);
			await UploadImagesAndSetToProduct(product, logo, background);

			context.Products.Update(product);
			await context.SaveChangesAsync();
			return await FindByIdAsync(product.Id);
		}

		private Product DeserializeProductAndMap(string productViewJson)
		{
			var productView = JsonConvert.DeserializeObject<ProductViewModel>(productViewJson);
			return mapper.Map<Product>(productView);
		}

		private async Task UploadImagesAndSetToProduct(Product product, IFormFile logo, IFormFile background)
		{
			var logoLinkTask = logo == null ? null : imageUploader.UploadAsync(logo);

			if (logoLinkTask != null)
				product.Logo = await logoLinkTask;

			var backgroundLinkTask = background == null ? null : imageUploader.UploadAsync(logo);

			if (backgroundLinkTask != null)
				product.Background = await backgroundLinkTask;
		}

		public async Task Delete(Guid id)
		{
			var product = await context.Products.FindAsync(id);
			if (product == null)
				return;

			product.DeletedAt = DateTime.Now;
			await context.SaveChangesAsync();
		}
	}
}
