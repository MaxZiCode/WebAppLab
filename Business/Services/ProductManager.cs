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
			var productView = JsonConvert.DeserializeObject<ProductViewModel>(productViewJson);
			var product = mapper.Map<Product>(productView);

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
			var product = await context.Products.FindAsync(id);
			if (product == null)
				return;

			product.DeletedAt = DateTime.Now;
			await context.SaveChangesAsync();
		}
	}
}
