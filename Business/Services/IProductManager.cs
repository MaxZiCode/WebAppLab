using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Data.Constants;
using WebAppLab.Models;

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
	}
}