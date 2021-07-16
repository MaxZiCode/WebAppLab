using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppLab
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDefaultEmailSender(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<SmtpSettings>(configuration.GetSection(nameof(SmtpSettings)));
			services.AddTransient<IEmailSender, DefaultEmailSender>();
			return services;
		}
	}
}
