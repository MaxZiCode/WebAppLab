using CloudinaryDotNet;

using MailKit;
using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WebAppLab.Data.Models;
using WebAppLab.Models;
using WebAppLab.Services;

namespace WebAppLab.Infrastructure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAuthManager<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : IdentityDbContext<User, IdentityRole, string>
		{
			services
				.AddIdentity<User, IdentityRole>(options =>
				{
					options.User.AllowedUserNameCharacters = string.Empty;
					options.User.RequireUniqueEmail = false;

					options.Password.RequireDigit = false;
					options.Password.RequiredLength = 0;
					options.Password.RequiredUniqueChars = 0;
					options.Password.RequireLowercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
				})
				.AddEntityFrameworkStores<TDbContext>()
				.AddDefaultTokenProviders();

			services
				.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"))
				.Configure<AuthData>(configuration.GetSection("Admin"));

			services
				.AddScoped<MailTransport, SmtpClient>()
				.AddScoped<IEmailSender, EmailSender>()
				.AddScoped<IConfirmEmailSender, ConfirmEmailSender>()
				.AddScoped<IAuthManager, AuthManager>();

			return services;
		}

		public static IServiceCollection AddUserChanger(this IServiceCollection services)
		{
			services
				.AddAutoMapper(config => config.CreateMap<User, UserViewModel>().ReverseMap());

			services
				.AddScoped<IUserChanger, UserChanger>();

			return services;
		}

		public static IServiceCollection AddProductManager(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddAutoMapper(config =>
				{
					config.CreateMap<Product, ProductViewModel>().ReverseMap();
					config.CreateMap<ProductForm, Product>();
				});

			services
				.AddCloudinary(configuration);

			services
				.AddScoped<IProductManager, ProductManager>();

			return services;
		}

		public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
		{
			var cloudinarySection = configuration.GetSection("CloudinarySettings");
			var settings = cloudinarySection.Get<CloudinarySettings>();
			Account account = new(settings.CloudName, settings.ApiKey, settings.ApiSecret);

			services
				.Configure<CloudinarySettings>(cloudinarySection);

			services
				.AddSingleton(account)
				.AddScoped<Cloudinary>()
				.AddScoped<IImageManager, ImageManager>();

			return services;
		}

		public static IServiceCollection AddOrderManager(this IServiceCollection services)
		{
			services
				.AddScoped<IOrderManager, OrderManager>();

			return services;
		}
	}
}
