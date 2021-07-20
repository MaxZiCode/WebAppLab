using System;
using System.Text.RegularExpressions;

namespace WebAppLab.Helpers
{
	public static class CheckUtility
	{
		private static readonly Regex regexPassword = new(
			// Regex source: https://ihateregex.io/expr/password/.
			@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,24}$",
			RegexOptions.Compiled | RegexOptions.CultureInvariant,
			TimeSpan.FromMilliseconds(128));

		private static readonly Regex regexEmail = new(
			// Regex source: https://ihateregex.io/expr/email/.
			@"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+",
			RegexOptions.Compiled | RegexOptions.CultureInvariant,
			TimeSpan.FromMilliseconds(128));

		public static bool CheckPassword(string password) => regexPassword.IsMatch(password);

		public static bool CheckEmail(string email) => regexEmail.IsMatch(email);
	}
}
