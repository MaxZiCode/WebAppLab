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
		Task<Order> AddProductAsync(ClaimsPrincipal user, AddProductData data);
		Task BuyProductsAsync(ClaimsPrincipal user);
		Task DeleteProductsAsync(ClaimsPrincipal user, IEnumerable<Guid> productIds);
		Task<IEnumerable<ProductAmount>> GetProductsAsync(ClaimsPrincipal user, Guid? orderId = null);
		Task<bool> HasUserOrderAsync(ClaimsPrincipal user);
	}
}