using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Serilog;

using WebAppLab.Data;
using WebAppLab.Data.Models;
using WebAppLab.Helpers;
using WebAppLab.Infrastructure;
using WebAppLab.Models;
using WebAppLab.Services;

namespace WebAppLab
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
			services.AddIdentity<User, IdentityRole>(options =>
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
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();
			services.AddDatabaseDeveloperPageExceptionFilter();

			services.AddSwaggerGen(c =>
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "WebAppLab API",
					Description = "An API of my internship project."
				})
			);

			services.AddDefaultEmailSender(Configuration);
			services.Configure<AuthData>(Configuration.GetSection("Admin"));

			services.AddTransient<IConfirmEmailSender, ConfirmEmailSender>();
			services.AddScoped<IAuthManager, AuthManager>();

			services.AddAutoMapper(config =>
			{
				config.CreateMap<User, UserViewModel>().ReverseMap();
				config.CreateMap<Product, ProductViewModel>().ReverseMap();
				config.CreateMap<ProductForm, Product>();
			});

			services.AddControllers().AddNewtonsoftJson();

			services.AddScoped<IUserChanger, UserChanger>();
			services.AddScoped<IProductManager, ProductManager>();

			services.AddCloudinary(Configuration);
		}

		public void Configure(
			IApplicationBuilder app, IWebHostEnvironment env,
			RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IOptions<AuthData> adminOptions)
		{
			DbFiller.EnsureRolesCreatedAsync(roleManager).Wait();
			DbFiller.EnsureAdminCreatedAsync(userManager, adminOptions).Wait();

			if (env.IsDevelopment())
			{
				app.UseSerilogRequestLogging();

				app.UseMigrationsEndPoint();

				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppLab API V1");
					c.RoutePrefix = string.Empty;
				});
			}
			else
			{
				app.UseExceptionLogging();
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
