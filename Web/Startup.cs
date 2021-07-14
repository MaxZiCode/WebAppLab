using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Serilog;

using WebAppLab.Data;

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
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Web")));
			services.AddIdentity<IdentityUser, IdentityRole>(options =>
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
				.AddEntityFrameworkStores<ApplicationDbContext>();
			services.AddDatabaseDeveloperPageExceptionFilter();

			services.AddControllers();

			services.AddSwaggerGen(c =>
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "WebAppLab API",
					Description = "An API of my internship project."
				})
			);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager)
		{
			EnsureRolesCreated(roleManager);

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

		private void EnsureRolesCreated(RoleManager<IdentityRole> roleManager)
		{
			foreach (var role in new[] { "Admin", "User" })
			{
				var isRoleExists = roleManager.RoleExistsAsync(role).Result;
				if (!isRoleExists)
				{
					roleManager.CreateAsync(new IdentityRole(role)).Wait();
				}
			}
		}
	}
}
