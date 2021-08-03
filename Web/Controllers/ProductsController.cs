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

using WebAppLab.Data;
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
		public async Task<PagedResult<List<ProductViewModel>>> GetProducts(int pageNumber, int pageSize)
		{
			var products = await context.Products
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<ProductViewModel>(mapper.ConfigurationProvider)
				.ToListAsync();

			var response = new PagedResult<List<ProductViewModel>>(products, pageNumber, pageSize);

			var count = await context.Products.CountAsync();
			var totalPages = Convert.ToInt32(Math.Ceiling((double)count / pageSize));

			response.NextPage = pageNumber == totalPages ? null : linkGenerator.GetUriByAction(HttpContext, values: new { pageNumber = pageNumber + 1, pageSize });
			response.PreviousPage = pageNumber == 1 ? null : linkGenerator.GetUriByAction(HttpContext, values: new { pageNumber = pageNumber - 1, pageSize });
			response.FirstPage = linkGenerator.GetUriByAction(HttpContext, values: new { pageNumber = 1, pageSize });
			response.LastPage = linkGenerator.GetUriByAction(HttpContext, values: new { pageNumber = totalPages, pageSize });
			response.TotalPages = totalPages;
			response.TotalRecords = count;

			return response;
		}
	}
}
