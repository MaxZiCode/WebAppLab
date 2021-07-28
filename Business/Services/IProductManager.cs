using System.Collections.Generic;
using System.Threading.Tasks;

using WebAppLab.Data.Models;

namespace WebAppLab.Services
{
	public interface IProductManager
	{
		Task<List<Product>> FindByName(string name, int maxCount, int offset);
		Task<List<Platforms>> GetTop(int count);
	}
}