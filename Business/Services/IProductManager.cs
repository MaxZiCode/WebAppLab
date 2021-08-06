
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using WebAppLab.Data.Constants;
using WebAppLab.Data.Models;
using WebAppLab.Models;
using WebAppLab.Wrappers;

namespace WebAppLab.Services
{
	public interface IProductManager
	{
		Task<List<ProductViewModel>> FindByNameAsync(string name, int maxCount, int offset);
		Task<List<Platforms>> GetTopAsync(int count);
		Task<ProductViewModel> FindByIdAsync(Guid id);
		Task<ProductViewModel> CreateAsync(ProductForm productData);
		Task<ProductViewModel> UpdateAsync(ProductForm productData);
		Task DeleteAsync(Guid id);
		Task<ProductRating> UpdateTotalRatingAsync(string userId, RatingData data);
		Task<PagedResult<List<ProductViewModel>>> GetPagedResultAsync(ProductListData data);
	}
}