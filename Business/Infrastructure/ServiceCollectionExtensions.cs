using CloudinaryDotNet;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WebAppLab.Attributes;
using WebAppLab.Services;

namespace WebAppLab.Infrastructure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
			services.AddTransient<IEmailSender, EmailSender>();
			return services;
		}

		public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
		{
			var cloudinarySection = configuration.GetSection("CloudinarySettings");
			var settings = cloudinarySection.Get<CloudinarySettings>();
			services.Configure<CloudinarySettings>(cloudinarySection);

			Account account = new(settings.CloudName, settings.ApiKey, settings.ApiSecret);

			services.AddSingleton(account);
			services.AddTransient<Cloudinary>();
			services.AddTransient<IImageManager, ImageManager>();

			return services;
		}

		public static IServiceCollection AddPager(this IServiceCollection services)
		{
			services.AddScoped<IProductPager, ProductPager>();
			services.AddScoped<ValidationSortFilterAttribute>();

			return services;
		}
	}
}
