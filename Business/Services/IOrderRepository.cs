using System;
using System.Threading.Tasks;

using WebAppLab.Data.Models;

namespace WebAppLab.Services
{
	public interface IOrderRepository
	{
		Task AddAsync(Order order);
		Task<Order> GetAsync(Guid orderId, bool track = false, bool includeProductAmounts = false);
		Task<Order> GetForUserAsync(string userId, bool track = false, bool includeProductAmounts = false);
		void Remove(Order order);
		Task SaveAsync();
	}
}