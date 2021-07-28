using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAppLab.Services
{
	public class ProductManager : IProductManager
	{
		private readonly ApplicationDbContext context;

		public ProductManager(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<List<Platforms>> GetTop(int count)
		{
			return await context.Products
				.GroupBy(p => p.Platform)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.Take(count)
				.ToListAsync();
		}

		public async Task<List<Product>> FindByName(string name, int maxCount = int.MaxValue, int offset = 0)
		{
			return await context.Products
				.Where(p => p.Name.Contains(name))
				.Skip(offset)
				.Take(maxCount)
				.ToListAsync();
		}
	}
}
