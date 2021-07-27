using System.Collections.Generic;

using WebAppLab.Models;
using WebAppLab.Data.Models;

namespace WebAppLab.Services
{
	public interface IProductManager
	{
		IEnumerable<ProductViewModel> FindByName(string name, int maxCount, int offset);
		IEnumerable<Platforms> GetTop(int count);
	}
}