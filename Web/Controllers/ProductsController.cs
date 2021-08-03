using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Data.Constants;
using WebAppLab.Data.Models;
using WebAppLab.Models;
using WebAppLab.Services;
using WebAppLab.Wrappers;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly LinkGenerator linkGenerator;

		public ProductsController(ApplicationDbContext context, IMapper mapper, LinkGenerator linkGenerator)
		{
			this.context = context;
			this.mapper = mapper;
			this.linkGenerator = linkGenerator;
		}

		[HttpGet("list")]
		public async Task<PagedResult<List<ProductViewModel>>> GetProductsList(int pageNumber, int pageSize, string genre, Rating? age, string sortBy, bool isSortAsc = false)
		{
			IQueryable<Product> productsQuery = context.Products;

			if (genre != null)
				productsQuery = productsQuery.Where(p => p.Genre == genre);

			if (age != null)
				productsQuery = productsQuery.Where(p => p.Rating.Equals(age.Value));

			productsQuery = sortBy switch
			{
				SortBy.Price => isSortAsc ? productsQuery.OrderBy(p => p.Price) : productsQuery.OrderByDescending(p => p.Price),
				_ => isSortAsc ? productsQuery.OrderBy(p => p.TotalRating) : productsQuery.OrderByDescending(p => p.TotalRating),
			};
			var totalProducts = await productsQuery.CountAsync();

			var products = await productsQuery
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<ProductViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			var response = new PagedResult<List<ProductViewModel>>(products, pageNumber, pageSize);

			var totalPages = Convert.ToInt32(Math.Ceiling((double)totalProducts / pageSize));
			response.NextPage = pageNumber == totalPages ? null : linkGenerator.GetUriByAction(HttpContext, values: new { pageNumber = pageNumber + 1, pageSize, genre, age, sortBy, isSortAsc });
			response.PreviousPage = pageNumber == 1 ? null : linkGenerator.GetUriByAction(HttpContext, values: new { pageNumber = pageNumber - 1, pageSize, genre, age, sortBy, isSortAsc });
			response.FirstPage = linkGenerator.GetUriByAction(HttpContext, values: new { pageNumber = 1, pageSize, genre, age, sortBy, isSortAsc });
			response.LastPage = linkGenerator.GetUriByAction(HttpContext, values: new { pageNumber = totalPages, pageSize, genre, age, sortBy, isSortAsc });
			response.TotalPages = totalPages;
			response.TotalRecords = totalProducts;

			return response;
		}
	}
}
