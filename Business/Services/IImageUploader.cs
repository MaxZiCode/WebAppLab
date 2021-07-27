using System.IO;
using System.Threading.Tasks;

namespace WebAppLab.Services
{
	public interface IImageUploader
	{
		string Upload(string fileName, Stream stream);
	}
}