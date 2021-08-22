using Microsoft.EntityFrameworkCore;

using System;

using WebAppLab.Data;

namespace WebAppLab.Tests
{
	public abstract class DbRequiredTests
	{
		protected DbContextOptions<ApplicationDbContext> ContextOptions { get; }

		protected DbRequiredTests()
		{
			ContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			Seed();
		}

		private void Seed()
		{
			using var context = new ApplicationDbContext(ContextOptions);
			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
		}
	}
}
