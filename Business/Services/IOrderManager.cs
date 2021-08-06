using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Models;

namespace WebAppLab.Services
{
	public interface IOrderManager
	{
		Task<Order> AddProductAsync(string userId, AddProductData data);
		Task BuyProductsAsync(string userId);
		Task DeleteProductsAsync(string userId, IEnumerable<Guid> productIds);
		Task<IEnumerable<ProductAmount>> GetProductsAsync(string userId, Guid? orderId = null);
		Task<bool> HasUserOrderAsync(string userId);
	}
}