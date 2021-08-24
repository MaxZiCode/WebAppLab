using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;

namespace WebAppLab.Services
{
	public class OrderRepository : IOrderRepository
	{
		private readonly ApplicationDbContext context;

		public OrderRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<Order> GetForUserAsync(string userId, bool track = false, bool includeProductAmounts = false) =>
			await GetQuery(track, includeProductAmounts).FirstOrDefaultAsync(o => o.UserId == userId);

		public async Task<Order> GetAsync(Guid orderId, bool track = false, bool includeProductAmounts = false) =>
			await GetQuery(track, includeProductAmounts).FirstOrDefaultAsync(o => o.Id == orderId);

		private IQueryable<Order> GetQuery(bool track, bool includeProductAmounts)
		{
			IQueryable<Order> orders = context.Orders.Where(o => !o.IsCompleted);

			if (!track)
				orders = orders.AsNoTracking();

			if (includeProductAmounts)
				orders = orders.Include(o => o.ProductAmounts);

			return orders;
		}

		public async Task AddAsync(Order order) => await context.Orders.AddAsync(order);

		public void Remove(Order order) => context.Orders.Remove(order);

		public async Task SaveAsync() => await context.SaveChangesAsync();
	}
}
