using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

namespace WebAppLab.Infrastructure
{
	public class ExceptionLoggingMiddleware
	{
		private readonly RequestDelegate next;
		private readonly ILogger logger;

		public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
		{
			this.next = next;
			this.logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Global error.");
			}
		}
	}
}
