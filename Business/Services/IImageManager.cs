using Microsoft.AspNetCore.Http;

using System.IO;
using System.Threading.Tasks;

namespace WebAppLab.Services
{
	public interface IImageManager
	{
		Task<string> UploadAsync(IFormFile file);
		Task DeleteAsync(string url);
	}
}