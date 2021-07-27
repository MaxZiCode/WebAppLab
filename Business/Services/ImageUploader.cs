using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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
