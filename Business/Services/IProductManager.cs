using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Models;

namespace WebAppLab.Services
{
	public interface IProductManager
	{
		IEnumerable<Platforms> GetTop(int count);
		IEnumerable<ProductViewModel> FindAllByName(string name, int maxCount = int.MaxValue, int offset = 0);
		Task<ProductViewModel> FindByIdAsync(Guid id);
		Task<ProductViewModel> Create(string productViewJson, IFormFile logo, IFormFile background);
		Task<ProductViewModel> Update(string productViewJson, IFormFile logo, IFormFile background);
		Task Delete(Guid id);
	}
}