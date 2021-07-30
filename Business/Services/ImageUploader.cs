using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System.IO;
using System.Threading.Tasks;

using WebAppLab.Infrastructure;

namespace WebAppLab.Services
{
	public class ImageUploader : IImageUploader
	{
		private readonly Cloudinary cloudinary;
		private readonly string imagesCloudFolder;

		public ImageUploader(Cloudinary cloudinary, IOptions<CloudinarySettings> options)
		{
			this.cloudinary = cloudinary;
			imagesCloudFolder = options.Value.ImagesFolder;
		}

		public async Task<string> UploadAsync(IFormFile file)
		{
			//Cloudinary cloudinary = new Cloudinary(cloudinaryAccount);
			using var fileStream = file.OpenReadStream();
			ImageUploadParams imageUploadParams = new()
			{
				File = new(file.FileName, fileStream),
				Folder = imagesCloudFolder
			};
			var result = await cloudinary.UploadAsync(imageUploadParams);
			return result.Url.AbsoluteUri;
		}
	}
}
