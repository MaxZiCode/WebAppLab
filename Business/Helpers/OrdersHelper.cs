using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Models;

namespace WebAppLab.Helpers
{
	public static class OrdersHelper
	{
		public static async Task<Order> GetOrderForUserAsync(IQueryable<Order> orders, string userId, bool track = false, bool includeProductAmounts = false)
		{
			if (!track)
				orders = orders.AsNoTracking();

			if (includeProductAmounts)
				orders = orders.Include(o => o.ProductAmounts);

			var singleNotCompletedOrder = await orders.SingleOrDefaultAsync(o => !o.IsCompleted && o.UserId == userId);
			return singleNotCompletedOrder;
		}
	}
}
