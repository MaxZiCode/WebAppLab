using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Data.Constants;
using WebAppLab.Data.Models;
using WebAppLab.Extensions;
using WebAppLab.Models;
using WebAppLab.Wrappers;

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

		public async Task<ProductRating> UpdateTotalRatingAsync(string userId, RatingData data)
		{
			var productRating = await AddRatingAsync(userId, data.ProductId, data.Rating);

			var product = await context.Products.FindAsync(data.ProductId);
			var sum = await context.ProductRatings.Where(pr => pr.ProductId == data.ProductId).SumAsync(pr => pr.Rating);
			var count = await context.ProductRatings.CountAsync();

			product.TotalRating = count != 0 ? sum / count : default;
			await context.SaveChangesAsync();

			return productRating;
		}

		public async Task<ProductRating> AddRatingAsync(string userId, Guid productId, int ratingValue)
		{
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

			return productRating;
		}

		public async Task<PagedResult<List<ProductViewModel>>> GetPagesAsync(ProductListData data)
		{
			IQueryable<Product> productsQuery = context.Products;
			productsQuery = AddFilteringToQuery(productsQuery, data);
			productsQuery = AddSortingToQuery(productsQuery, data.SortBy, data.IsSortAsc ?? false);

			int totalProducts = await productsQuery.CountAsync();
			var pagedProducts = await productsQuery
				.Skip((data.PageNumber - 1) * data.PageSize)
				.Take(data.PageSize)
				.ProjectTo<ProductViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			return new PagedResult<List<ProductViewModel>>(pagedProducts, totalProducts, data.PageSize, data.PageNumber);
		}

		private IQueryable<Product> AddFilteringToQuery(IQueryable<Product> query, ProductListData data)
		{
			if (data.Genre != null)
			{
				return query.Where(p => p.Genre == data.Genre);
			}

			if (data.Age != null)
			{
				Rating dataAge = Enum.Parse<Rating>(data.Age);
				return query.Where(p => p.Rating == dataAge);
			}

			return query;
		}

		private IQueryable<Product> AddSortingToQuery(IQueryable<Product> query, string sort, bool isSortAsc)
		{
			SortBy sortBy = sort == null ? SortBy.TotalRating : Enum.Parse<SortBy>(sort, ignoreCase: true);
			return sortBy switch
			{
				SortBy.Price => AddSortingByProperty(p => p.Price),
				_ => AddSortingByProperty(p => p.TotalRating)
			};

			IQueryable<Product> AddSortingByProperty<T>(Expression<Func<Product, T>> keySelector) =>
				isSortAsc ?
					query.OrderBy(keySelector) :
					query.OrderByDescending(keySelector);
		}
	}
}
