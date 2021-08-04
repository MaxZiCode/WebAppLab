using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Models;
using WebAppLab.Services;
using WebAppLab.Wrappers;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductPager productPager;

		public ProductsController(IProductPager productPager)
		{
			this.productPager = productPager;
		}

		[HttpGet("list")]
		public async Task<PagedResult<List<ProductViewModel>>> GetProductsList([FromQuery] ProductListData data)
		{
			return await productPager.GetPagedResultAsync(HttpContext, data);
		}
	}
}
