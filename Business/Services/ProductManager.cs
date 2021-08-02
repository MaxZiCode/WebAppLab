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
using System.Security.Claims;
using WebAppLab.Extensions;

namespace WebAppLab.Services
{
	public class ProductManager : IProductManager
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly IImageManager imageManager;

		public ProductManager(ApplicationDbContext context, IMapper mapper, IImageManager imageManager)
		{
			this.context = context;
			this.mapper = mapper;
			this.imageManager = imageManager;
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
			Product product = mapper.Map<Product>(productData);
			await context.Products.AddAsync(product);

			var logoLinkTask = UploadImageAsync(productData.Logo);
			var backgroundLinkTask = UploadImageAsync(productData.Background);
			product.Logo = await logoLinkTask;
			product.Background = await backgroundLinkTask;

			context.Update(product);
			await context.SaveChangesAsync();
			return mapper.Map<ProductViewModel>(product);

		}

		public async Task<ProductViewModel> UpdateAsync(ProductForm productData)
		{
			var product = context.Products.Find(productData.Id);

			var logoDeleteTask = imageManager.DeleteAsync(product.Logo);
			var backgroundDeleteTask = imageManager.DeleteAsync(product.Background);
			var logoLinkTask = UploadImageAsync(productData.Logo);
			var backgroundLinkTask = UploadImageAsync(productData.Background);

			mapper.Map(productData, product);

			await logoDeleteTask;
			await backgroundDeleteTask;
			product.Logo = await logoLinkTask;
			product.Background = await backgroundLinkTask;

			context.Products.Update(product);
			await context.SaveChangesAsync();
			return mapper.Map<ProductViewModel>(product);
		}

		private async Task<string> UploadImageAsync(IFormFile formFile)
		{
			if (formFile == null)
				return null;

			return await imageManager.UploadAsync(formFile);
		}

		public async Task DeleteAsync(Guid id)
		{
			var product = await context.Products.FindAsync(id);
			if (product == null)
				return;

			await imageManager.DeleteAsync(product.Logo);
			await imageManager.DeleteAsync(product.Background);

			product.DeletedAt = DateTime.Now;
			await context.SaveChangesAsync();
		}

		public async Task<ProductRating> AddProductRatingAsync(ClaimsPrincipal userClaims, Guid productId, int ratingValue)
		{
			var userId = userClaims.GetId();
			ProductRating productRating = new()
			{
				ProductId = productId,
				UserId = userId,
				Rating = ratingValue
			};

			if (!await context.ProductRatings.ContainsAsync(productRating))
			{
				await context.ProductRatings.AddAsync(productRating);
			}
			else
			{
				context.ProductRatings.Update(productRating);
			}

			await context.SaveChangesAsync();

			return await context.ProductRatings.FindAsync(productId, userId);
		}

		public async Task UpdateTotalRatingAsync(Guid productId)
		{
			var product = await context.Products.FindAsync(productId);
			var sum = await context.ProductRatings.Where(pr => pr.ProductId == productId).SumAsync(pr => pr.Rating);
			var count = await context.ProductRatings.CountAsync();

			product.TotalRating = count != 0 ? sum / count : default;
			context.Update(product);
			await context.SaveChangesAsync();
		}
	}
}
