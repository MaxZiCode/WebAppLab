using System.Collections.Generic;

using WebAppLab.Data.Models;

namespace WebAppLab.Services
{
	public interface IProductManager
	{
		IEnumerable<Product> FindByName(string name, int maxCount, int offset);
		IEnumerable<Platforms> GetTop(int count);
	}
}