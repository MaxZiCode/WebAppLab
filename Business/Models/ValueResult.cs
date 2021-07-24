namespace WebAppLab.Models
{
	public class ValueResult<T> : Result
	{
		public T Value { get; protected set; }

		public static ValueResult<T> FromValue(T value) => new() { Succeeded = true, Value = value };
		public static new ValueResult<T> FromFail(string failMessage) => new() { Succeeded = false, Value = default, FailMessage = failMessage };
	}
}
