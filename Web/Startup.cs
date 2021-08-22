using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
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
			services
				.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")))
				.AddDatabaseDeveloperPageExceptionFilter();

			services
				.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "WebAppLab API",
					Description = "An API of my internship project."
				}))
				.AddResponseCompression(options =>
				{
					options.Providers.Add<GzipCompressionProvider>();
					options.EnableForHttps = true;
				})
				.AddMemoryCache();

			services
				.AddControllers()
				.AddNewtonsoftJson();

			services
				.AddAuthManager<ApplicationDbContext>(Configuration)
				.AddUserChanger()
				.AddProductManager(Configuration)
				.AddOrderManager();
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

			app.UseResponseCompression();

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
