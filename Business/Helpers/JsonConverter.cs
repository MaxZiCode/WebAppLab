using Newtonsoft.Json;

namespace WebAppLab.Helpers
{
	public static class JsonConverter
	{
		// JsonConverter from System.Text.Json have a DateTime convert issue.
		public static T GetObject<T>(string json) => JsonConvert.DeserializeObject<T>(json);
	}
}
