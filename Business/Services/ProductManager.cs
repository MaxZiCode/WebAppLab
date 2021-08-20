using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Constants;
using WebAppLab.Data.Models;
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
				.AsNoTracking()
				.GroupBy(p => p.Platform)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.Take(count)
				.ToListAsync();
		}

		public async Task<List<ProductViewModel>> FindByNameAsync(string name, int maxCount = int.MaxValue, int offset = 0)
		{
			return await context.Products
				.AsNoTracking()
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

			var logoLink = await UploadImageAsync(productData.Logo);
			var backgroundLink = await UploadImageAsync(productData.Background);
			product.Logo = logoLink;
			product.Background = backgroundLink;

			await context.Products.AddAsync(product);
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

		public async Task<ProductRating> AddRatingAndUpdateTotalAsync(string userId, RatingData data)
		{
			ProductRating productRating = new()
			{
				ProductId = data.ProductId,
				UserId = userId,
				Rating = data.Rating
			};

			var porduct = await context.Products.Include(p => p.ProductRatings).FirstAsync(p => p.Id == data.ProductId);

			var existingRating = porduct.ProductRatings.FirstOrDefault(r => r.UserId == userId && r.ProductId == data.ProductId);
			if (existingRating == null)
				porduct.ProductRatings.Add(productRating);
			else
				existingRating.Rating = data.Rating;

			UpdateTotalRatingAsync(porduct);
			await context.SaveChangesAsync();

			return productRating;
		}

		private void UpdateTotalRatingAsync(Product product)
		{
			var sum = product.ProductRatings.Where(pr => pr.ProductId == product.Id).Sum(pr => pr.Rating);
			var count = product.ProductRatings.Count();

			product.TotalRating = sum / count;
		}

		public async Task<PagedResult<List<ProductViewModel>>> GetPagesAsync(ProductListData data)
		{
			IQueryable<Product> productsQuery = context.Products.AsNoTracking();
			productsQuery = AddFilteringToQuery(productsQuery, data);
			productsQuery = AddSortingToQuery(productsQuery, data.SortBy ?? SortBy.TotalRating, data.IsSortAsc ?? false);

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
				return query.Where(p => p.Genre == data.Genre);

			if (data.Age != null)
				return query.Where(p => p.Rating == data.Age);

			return query;
		}

		private IQueryable<Product> AddSortingToQuery(IQueryable<Product> query, SortBy sort, bool isSortAsc)
		{
			return sort switch
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
