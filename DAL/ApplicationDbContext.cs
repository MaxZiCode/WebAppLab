using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System.Reflection;

using WebAppLab.Data.Configurations;

using WebAppLab.Data.Models;

namespace WebAppLab.Data
{
	public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
	{
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductRating> ProductRatings { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<ProductAmount> ProductAmounts { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
