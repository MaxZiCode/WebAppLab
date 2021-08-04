using AutoMapper;

using Microsoft.AspNetCore.Routing;

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
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;

namespace WebAppLab.Services
{
	public class ProductPager : IProductPager
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly LinkGenerator linkGenerator;

		private HttpContext httpContext;
		private ProductListData data;

		public ProductPager(ApplicationDbContext context, IMapper mapper, LinkGenerator linkGenerator)
		{
			this.context = context;
			this.mapper = mapper;
			this.linkGenerator = linkGenerator;
		}

		public async Task<PagedResult<List<ProductViewModel>>> GetPagedResultAsync(HttpContext httpContext, ProductListData data)
		{
			this.httpContext = httpContext;
			this.data = data;

			IQueryable<Product> productsQuery = context.Products;
			productsQuery = AddFilteringToQuery(productsQuery);
			productsQuery = AddSortingToQuery(productsQuery);

			int totalProducts = await productsQuery.CountAsync();
			var pagedProducts = await productsQuery
				.Skip((data.PageNumber - 1) * data.PageSize)
				.Take(data.PageSize)
				.ProjectTo<ProductViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			var result = CreatePagedResult(pagedProducts, totalProducts);

			return result;
		}

		private IQueryable<Product> AddFilteringToQuery(IQueryable<Product> query)
		{
			if (data.Genre != null)
				return query.Where(p => p.Genre == data.Genre);

			if (data.Age != null)
				return query.Where(p => p.Rating == data.Age.Value);

			return query;
		}

		private IQueryable<Product> AddSortingToQuery(IQueryable<Product> query)
		{
			return data.SortBy switch
			{
				SortBy.Price => AddSortingByProperty(query, p => p.Price),
				// Sorting must be anyway for Skip and Take.
				_ => AddSortingByProperty(query, p => p.TotalRating)
			};
		}

		private IQueryable<Product> AddSortingByProperty<T>(IQueryable<Product> query, Expression<Func<Product, T>> keySelector) =>
				data.IsSortAsc ?? false ?
					query.OrderBy(keySelector) :
					query.OrderByDescending(keySelector);

		private PagedResult<List<ProductViewModel>> CreatePagedResult(List<ProductViewModel> pagedProducts, int totalProducts)
		{
			var result = new PagedResult<List<ProductViewModel>>(pagedProducts, data.PageNumber, data.PageSize);

			var totalPages = Convert.ToInt32(Math.Ceiling((double)totalProducts / data.PageSize));
			result.NextPage = data.PageNumber == totalPages ? null : GenerateLinkToPage(data.PageNumber + 1);
			result.PreviousPage = data.PageNumber == 1 ? null : GenerateLinkToPage(data.PageNumber - 1);
			result.FirstPage = GenerateLinkToPage(1);
			result.LastPage = GenerateLinkToPage(totalPages);
			result.TotalPages = totalPages;
			result.TotalRecords = totalProducts;

			return result;
		}

		private string GenerateLinkToPage(int page)
		{
			var dataClone = data.Clone();
			dataClone.PageNumber = page;

			return linkGenerator.GetUriByAction(httpContext, values: dataClone);
		}
	}
}
