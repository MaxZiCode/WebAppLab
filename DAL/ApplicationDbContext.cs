using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using WebAppLab.Data.Models;

namespace WebAppLab.Data
{
	public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
	{
		public DbSet<Product> Products { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Product>().Property(p => p.Platform).HasConversion<int>();
		}
	}
}
