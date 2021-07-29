using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.Extensions.Options;

using System.IO;

using WebAppLab.Infrastructure;

namespace WebAppLab.Services
{
	public class ImageUploader : IImageUploader
	{
		private readonly Cloudinary cloudinary;
		private readonly string imagesFolder;

		public ImageUploader(Cloudinary cloudinary, IOptions<CloudinarySettings> options)
		{
			this.cloudinary = cloudinary;
			imagesFolder = options.Value.ImagesFolder;
		}

		public string Upload(string fileName, Stream stream)
		{
			ImageUploadParams imageUploadParams = new()
			{
				File = new(fileName, stream),
				Folder = imagesFolder
			};
			// UploadAsync method have an issue with Stream disposing.
			var result = cloudinary.Upload(imageUploadParams);
			return result.Url.AbsoluteUri;
		}
	}
}
