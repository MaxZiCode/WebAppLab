using System.Collections.Generic;

using WebAppLab.Models;

namespace WebAppLab.Services
{
	public interface IProductManager
	{
		IEnumerable<ProductViewModel> FindByName(string name, int maxCount, int offset);
		IEnumerable<string> GetTop(int count);
	}
}