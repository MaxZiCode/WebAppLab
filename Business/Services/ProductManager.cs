using AutoMapper;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Models;

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

		public IEnumerable<Platforms> GetTop(int count)
		{
			return context.Products
				.AsParallel()
				.GroupBy(p => p.Platform)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.Take(count);
		}

		public IEnumerable<ProductViewModel> FindAllByName(string name, int maxCount = int.MaxValue, int offset = 0)
		{
			return context.Products
				.AsParallel()
				.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
				.Skip(offset)
				.Take(maxCount)
				.Select(p => ProductToView(p));
		}

		public async Task<ProductViewModel> FindByIdAsync(Guid id)
		{
			var product = await FindInDbAsync(id);
			return ProductToView(product);
		}

		public async Task<ProductViewModel> Create(string productViewJson, IFormFile logo, IFormFile background) =>
			await ProductAction(async p => await context.Products.AddAsync(p), productViewJson, logo, background);

		public async Task<ProductViewModel> Update(string productViewJson, IFormFile logo, IFormFile background) =>
			await ProductAction(p =>
			{
				context.Products.Update(p);
				return Task.CompletedTask;
			}, productViewJson, logo, background);

		private async Task<ProductViewModel> ProductAction(Func<Product, Task> action, string productViewJson, IFormFile logo, IFormFile background)
		{
			var productView = JsonConverter.GetObject<ProductViewModel>(productViewJson);
			var product = ViewToProduct(productView);

			var result = Parallel.ForEach(new (IFormFile File, Action<string> Action)[]
			{
				(logo, s => product.Logo = s),
				(background,  s => product.Background = s)
			},
			item =>
			{
				if (item.File != null)
				{
					var fileName = item.File.FileName;
					using var stream = item.File.OpenReadStream();
					var link = imageUploader.Upload(item.File.FileName, stream);
					item.Action(link);
				}
			});

			await action(product);
			await context.SaveChangesAsync();
			return await FindByIdAsync(product.Id);
		}

		public async Task Delete(Guid id)
		{
			var product = await FindInDbAsync(id);
			if (product == null)
				return;

			product.DeletedAt = DateTime.Now;
			await context.SaveChangesAsync();
		}

		private async Task<Product> FindInDbAsync(Guid id)
		{
			return await context.Products.FindAsync(id);
		}

		private ProductViewModel ProductToView(Product product) => mapper.Map<ProductViewModel>(product);

		private Product ViewToProduct(ProductViewModel productView) => mapper.Map<Product>(productView);
	}
}
