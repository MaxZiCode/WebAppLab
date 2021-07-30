using Microsoft.AspNetCore.Http;

using System.IO;
using System.Threading.Tasks;

namespace WebAppLab.Services
{
	public interface IImageUploader
	{
		Task<string> UploadAsync(IFormFile file);
	}
}