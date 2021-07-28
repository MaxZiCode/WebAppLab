using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Data.Models;
using WebAppLab.Data.Constants;

namespace WebAppLab.Services
{
	public interface IProductManager
	{
		Task<List<Product>> FindByName(string name, int maxCount, int offset);
		Task<List<Platforms>> GetTop(int count);
	}
}