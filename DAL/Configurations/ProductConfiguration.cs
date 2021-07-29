using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebAppLab.Data.Models;

namespace WebAppLab.Data.Configurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.HasIndex(p => p.Name);
			builder.HasIndex(p => p.Platform);
			builder.HasIndex(p => p.DateCreated);
			builder.HasIndex(p => p.TotalRating);

			builder.Property(p => p.Name)
				.IsRequired();

			builder.Property(p => p.Platform)
				.HasConversion<int>()
				.IsRequired();

			builder.Property(p => p.DateCreated)
				.HasColumnType("Date")
				.IsRequired();

			builder.Property(p => p.Rating)
				.HasConversion<int>();

			builder.Property(p => p.Price)
				.HasColumnType("Money");

			builder.Property(p => p.DeletedAt)
				.HasColumnType("Date")
				.IsRequired(false);

			builder.HasQueryFilter(p => p.DeletedAt == null);
		}
	}
}
