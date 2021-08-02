using AutoMapper;
using AutoMapper.QueryableExtensions;

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

		public async Task<List<Platforms>> GetTopAsync(int count)
		{
			return await context.Products
				.GroupBy(p => p.Platform)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.Take(count)
				.ToListAsync();
		}

		public async Task<List<ProductViewModel>> FindByNameAsync(string name, int maxCount = int.MaxValue, int offset = 0)
		{
			return await context.Products
				.Where(p => p.Name.Contains(name))
				.OrderBy(p => p.Name)
				.Skip(offset)
				.Take(maxCount)
				.ProjectTo<ProductViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();
		}

		public async Task<ProductViewModel> FindByIdAsync(Guid id)
		{
			var product = await context.Products.FindAsync(id);
			return mapper.Map<ProductViewModel>(product);
		}

		public async Task<ProductViewModel> CreateAsync(ProductForm productData)
		{
			var product = await UploadImagesAndGetProductAsync(productData);

			await context.Products.AddAsync(product);
			await context.SaveChangesAsync();
			return mapper.Map<ProductViewModel>(product);

		}

		public async Task<ProductViewModel> UpdateAsync(ProductForm productData)
		{
			var product = await UploadImagesAndGetProductAsync(productData);

			context.Products.Update(product);
			await context.SaveChangesAsync();
			return mapper.Map<ProductViewModel>(product);
		}

		private async Task<Product> UploadImagesAndGetProductAsync(ProductForm productData)
		{
			var logoLinkTask = UploadImageAsync(productData.Logo);
			var backgroundLinkTask = UploadImageAsync(productData.Background);

			Product product = mapper.Map<Product>(productData);

			product.Logo = await logoLinkTask;
			product.Background = await backgroundLinkTask;

			return product;
		}

		private async Task<string> UploadImageAsync(IFormFile formFile)
		{
			if (formFile == null)
				return null;

			return await imageUploader.UploadAsync(formFile);
		}

		public async Task DeleteAsync(Guid id)
		{
			var product = await context.Products.FindAsync(id);
			if (product == null)
				return;

			product.DeletedAt = DateTime.Now;
			await context.SaveChangesAsync();
		}
	}
}
