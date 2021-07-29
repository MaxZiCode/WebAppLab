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
		Task<List<ProductViewModel>> FindByName(string name, int maxCount, int offset);
		Task<List<Platforms>> GetTop(int count);
		Task<ProductViewModel> FindByIdAsync(Guid id);
		Task<ProductViewModel> Create(string productViewJson, IFormFile logo, IFormFile background);
		Task<ProductViewModel> Update(string productViewJson, IFormFile logo, IFormFile background);
		Task Delete(Guid id);
	}
}