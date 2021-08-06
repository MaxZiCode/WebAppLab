using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Models;
using WebAppLab.Wrappers;

namespace WebAppLab.Services
{
	public class ProductPager : IProductPager
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;

		public ProductPager(ApplicationDbContext context, IMapper mapper)
		{
			this.context = context;
			this.mapper = mapper;
		}

		public async Task<PagedResult<List<ProductViewModel>>> GetPagedResultAsync(ProductListData data)
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
				return query.Where(p => p.Genre == data.Genre);

			if (data.Age != null)
				return query.Where(p => p.Rating == data.Age.Value);

			return query;
		}

		private IQueryable<Product> AddSortingToQuery(IQueryable<Product> query, string sort, bool isSortAsc)
		{
			return sort switch
			{
				SortBy.Price => AddSortingByProperty(p => p.Price),
				// Sorting must be anyway for Skip and Take.
				_ => AddSortingByProperty(p => p.TotalRating)
			};

			IQueryable<Product> AddSortingByProperty<T>(Expression<Func<Product, T>> keySelector) =>
				isSortAsc ?
					query.OrderBy(keySelector) :
					query.OrderByDescending(keySelector);
		}
	}
}
