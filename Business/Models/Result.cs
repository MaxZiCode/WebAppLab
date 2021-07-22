namespace WebAppLab.Models
{
	public class Result
	{
		public bool Succeeded { get; protected set; }
		public string FailMessage { get; protected set; }

		public static Result Success => new() { Succeeded = true };

		public static Result FromFail(string message) => new() { Succeeded = false, FailMessage = message };
	}
}
