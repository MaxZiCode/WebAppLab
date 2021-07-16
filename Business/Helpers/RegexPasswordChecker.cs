using System;
using System.Text.RegularExpressions;

namespace WebAppLab.Helpers
{
	public class RegexPasswordChecker : IPasswordChecker
	{
		private readonly Regex regex = new(
			// Regex source: https://ihateregex.io/expr/password/.
			@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,24}$",
			RegexOptions.Compiled | RegexOptions.CultureInvariant,
			TimeSpan.FromMilliseconds(128));

		public bool Check(string password)
		{
			return regex.IsMatch(password);
		}
	}
}
