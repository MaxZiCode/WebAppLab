using Microsoft.AspNetCore.Builder;

namespace WebAppLab.Infrastructure
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseExceptionLogging(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionLoggingMiddleware>();
	}
}
