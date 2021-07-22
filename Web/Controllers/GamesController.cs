using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;

namespace WebAppLab.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GamesController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public GamesController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet("topPlatforms")]
		public IEnumerable<string> GetTopPlatforms()
		{
			return context.Products.AsParallel().GroupBy(p => p.Platform).OrderByDescending(g => g.Count()).Select(g => g.Key.ToString()).Take(3);
		}

		[HttpGet("search")]
		public IEnumerable<Product> SearchProducts(string term, int limit, int offset)
		{
			return context.Products.AsParallel().Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)).Skip(offset).Take(limit);
		}
	}
}
