namespace WebAppLab.Wrappers
{
	public class Result
	{
		public bool Succeeded { get; protected set; }

		public string[] FailMessages { get; protected set; }
		public string FailMessage => FailMessages == null ? null : string.Join(' ', FailMessages);

		public static Result Success => new() { Succeeded = true };

		public static Result FromFail(params string[] messages) => new() { Succeeded = false, FailMessages = messages };
	}
}
