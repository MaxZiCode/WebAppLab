using Microsoft.AspNetCore.Http;

using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Models;
using WebAppLab.Wrappers;

namespace WebAppLab.Services
{
	public interface IProductPager
	{
		Task<PagedResult<List<ProductViewModel>>> GetPagedResultAsync(HttpContext httpContext, ProductListData data);
	}
}