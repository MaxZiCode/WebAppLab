using System.Linq;
using System.Security.Claims;

namespace WebAppLab.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static string GetId(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
	}
}
