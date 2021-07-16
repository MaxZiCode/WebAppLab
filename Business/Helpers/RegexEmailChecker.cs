using System;
using System.Text.RegularExpressions;

namespace WebAppLab.Helpers
{
	public class RegexEmailChecker : IEmailChecker
	{
		private readonly Regex regex = new(
			// Regex source: https://ihateregex.io/expr/email/.
			@"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+",
			RegexOptions.Compiled | RegexOptions.CultureInvariant,
			TimeSpan.FromMilliseconds(128));

		public bool Check(string email)
		{
			return regex.IsMatch(email);
		}
	}
}
