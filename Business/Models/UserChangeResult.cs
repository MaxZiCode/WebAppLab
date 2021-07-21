namespace WebAppLab.Models
{
	public class UserChangeResult
	{
		public bool Succeeded { get; private set; }
		public User User { get; private set; }
		public string Error { get; private set; }

		public static UserChangeResult FromUser(User user) => new() { Succeeded = true, User = user };
		public static UserChangeResult FromError(string error) => new() { Succeeded = false, Error = error };
	}
}
