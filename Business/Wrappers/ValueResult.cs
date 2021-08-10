namespace WebAppLab.Wrappers
{
	public class ValueResult<T> : Result
	{
		public T Data { get; protected set; }

		public static ValueResult<T> FromValue(T value) => new() { Succeeded = true, Data = value };
		public static new ValueResult<T> FromFail(params string[] messages) => new() { Succeeded = false, Data = default, FailMessages = messages };
	}
}
