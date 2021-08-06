using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Attributes;
using WebAppLab.Models;
using WebAppLab.Services;
using WebAppLab.Wrappers;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductManager productManager;

		public ProductsController(IProductManager productManager)
		{
			this.productManager = productManager;
		}

		[HttpGet("list")]
		[ServiceFilter(typeof(ValidationSortFilterAttribute))]
		public async Task<PagedResult<List<ProductViewModel>>> GetProductsList([FromQuery] ProductListData data)
		{
			return await productManager.GetPagedResultAsync(data);
		}
	}
}
