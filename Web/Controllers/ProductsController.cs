using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
		private readonly IProductManager productManager;

		public ProductsController(IProductManager productManager)
		{
			this.productManager = productManager;
		}

		[HttpGet("list")]
		public async Task<IActionResult> GetProductsList([FromQuery] ProductListData data)
		{
			return Ok(await productManager.GetPagesAsync(data));
		}
	}
}
