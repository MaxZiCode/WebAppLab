using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Models;

namespace WebAppLab.Services
{
	public class ProductManager : IProductManager
	{
		private readonly ApplicationDbContext context;
		private readonly IMapper mapper;

		public ProductManager(ApplicationDbContext context, IMapper mapper)
		{
			this.context = context;
			this.mapper = mapper;
		}

		public IEnumerable<string> GetTop(int count)
		{
			return context.Products
				.AsParallel()
				.GroupBy(p => p.Platform)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key.ToString())
				.Take(count);
		}

		public IEnumerable<ProductViewModel> FindByName(string name, int maxCount = int.MaxValue, int offset = 0)
		{
			return context.Products
				.AsParallel()
				.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
				.Skip(offset)
				.Take(maxCount)
				.Select(p => mapper.Map<ProductViewModel>(p));
		}
	}
}
