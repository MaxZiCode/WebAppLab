using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebAppLab.Data;

namespace WebAppLab.Tests
{
	public abstract class DbRequiredTests
	{
		protected DbContextOptions<ApplicationDbContext> ContextOptions { get; }

		protected DbRequiredTests()
		{
			ContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase("Default")
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
