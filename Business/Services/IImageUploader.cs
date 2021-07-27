using System.IO;

namespace WebAppLab.Services
{
	public interface IImageUploader
	{
		string Upload(string fileName, Stream stream);
	}
}