using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using WebAppLab.Infrastructure;

namespace WebAppLab.Services
{
	public class ImageManager : IImageManager
	{
		private readonly Cloudinary cloudinary;
		private readonly string imagesCloudFolder;

		public ImageManager(Cloudinary cloudinary, IOptions<CloudinarySettings> options)
		{
			this.cloudinary = cloudinary;
			imagesCloudFolder = options.Value.ImagesFolder;
		}

		public async Task<string> UploadAsync(IFormFile formFile)
		{
			using var formFileStream = formFile.OpenReadStream();
			ImageUploadParams imageUploadParams = new()
			{
				File = new(formFile.FileName, formFileStream),
				Folder = imagesCloudFolder
			};
			var result = await cloudinary.UploadAsync(imageUploadParams);
			return result.Url.AbsoluteUri;
		}

		public async Task DeleteAsync(string url)
		{
			var id = GetId(url);
			await cloudinary.DestroyAsync(new(id));
		}

		private string GetId(string url)
		{
			// Url example: res.cloudinary.com/.../{id}.jpg

			int begin = url.LastIndexOf('/');
			int end = url.LastIndexOf('.');
			int length = end - begin;
			string idWithoutFolder = url.Substring(begin, length);
			return new StringBuilder(imagesCloudFolder).Append(idWithoutFolder).ToString();
		}
	}
}
