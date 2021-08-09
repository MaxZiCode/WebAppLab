using System;
using System.Linq;
using System.Text.RegularExpressions;

using WebAppLab.Data;

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

		private static readonly Regex regexPhoneNumber = new(
			// Examples: +375 (29) 278-75-67
			//			 375292787567
			@"^\+?[0-9]{3}\s?\(?[0-9]{2}\)?\s?[0-9]{3}[-\s]?[0-9]{2}[-\s]?[0-9]{2}$",
			RegexOptions.Compiled | RegexOptions.CultureInvariant,
			TimeSpan.FromMilliseconds(128));

		public static bool CheckPassword(string password) => regexPassword.IsMatch(password);

		public static bool CheckEmail(string email) => regexEmail.IsMatch(email);

		public static bool CheckPhoneNumber(string phoneNumber) => regexPhoneNumber.IsMatch(phoneNumber);
	}
}
