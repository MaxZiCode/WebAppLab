using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Data.Constants;
using WebAppLab.Services;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GamesController : ControllerBase
	{
		private readonly IProductManager productManager;

		public GamesController(IProductManager productManager)
		{
			this.productManager = productManager;
		}

		[HttpGet("topPlatforms")]
		public async Task<List<Platforms>> GetTopPlatforms() => await productManager.GetTop(3);

		[HttpGet("search")]
		public async Task<List<Product>> SearchProducts(string term, int limit, int offset) => await productManager.FindByName(term, limit, offset);
	}
}
