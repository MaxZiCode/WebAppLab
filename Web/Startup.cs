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

using WebAppLab.Constants;
using WebAppLab.Data;
using WebAppLab.Infrastructure;
using WebAppLab.Models;

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
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();
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

			services.AddDefaultEmailSender(Configuration);
			services.Configure<AuthData>(Configuration.GetSection("Admin"));
		}

		public void Configure(
			IApplicationBuilder app,
			IWebHostEnvironment env,
			RoleManager<IdentityRole> roleManager,
			UserManager<IdentityUser> userManager,
			IOptions<AuthData> adminOptions)
		{
			EnsureRolesCreated(roleManager);
			EnsureAdminCreated(userManager, adminOptions);

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
			foreach (var role in new[] { Roles.Admin, Roles.User })
			{
				var isRoleExists = roleManager.RoleExistsAsync(role).Result;
				if (!isRoleExists)
				{
					roleManager.CreateAsync(new IdentityRole(role)).Wait();
				}
			}
		}

		private void EnsureAdminCreated(UserManager<IdentityUser> userManager, IOptions<AuthData> adminOptions)
		{
			var adminData = adminOptions.Value;

			var user = userManager.FindByEmailAsync(adminData.Email).Result;
			if (user == null)
			{
				var admin = new IdentityUser() { Email = adminData.Email, UserName = adminData.Email };
				userManager.CreateAsync(admin, adminData.Password).Wait();
				userManager.AddToRoleAsync(admin, Roles.Admin).Wait();
			}
		}
	}
}
