using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;

namespace WebAppLab.Services
{
	public class ProductManager : IProductManager
	{
		private readonly ApplicationDbContext context;

		public ProductManager(ApplicationDbContext context)
		{
			this.context = context;
		}

		public IEnumerable<Platforms> GetTop(int count)
		{
			return context.Products
				.AsParallel()
				.GroupBy(p => p.Platform)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.Take(count);
		}

		public IEnumerable<Product> FindByName(string name, int maxCount = int.MaxValue, int offset = 0)
		{
			return context.Products
				.AsParallel()
				.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
				.Skip(offset)
				.Take(maxCount);
		}
	}
}
