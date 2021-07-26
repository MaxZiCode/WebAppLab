using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WebAppLab.Services;
using CloudinaryDotNet;

namespace WebAppLab.Infrastructure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDefaultEmailSender(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<SmtpSettings>(configuration.GetSection(nameof(SmtpSettings)));
			services.AddTransient<IEmailSender, EmailSender>();
			return services;
		}

		public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
		{
			var settings = configuration.GetSection(nameof(CloudinarySettings)).Get<CloudinarySettings>();
			Account account = new(settings.CloudName, settings.ApiKey, settings.ApiSecret);
			var cloudinary = new Cloudinary(account);
			cloudinary.Api.Secure = true;

			services.AddSingleton(cloudinary);

			return services;
		}
	}
}
