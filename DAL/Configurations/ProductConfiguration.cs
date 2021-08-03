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
			builder.HasIndex(p => p.Genre);
			builder.HasIndex(p => p.Rating);
			builder.HasIndex(p => p.Price);

			builder.Property(p => p.Name)
				.IsRequired();

			builder.Property(p => p.Platform)
				.IsRequired();

			builder.Property(p => p.DateCreated)
				.HasColumnType("Date")
				.IsRequired();

			builder.Property(p => p.Price)
				.HasColumnType("DECIMAL(16,4)");

			builder.Property(p => p.DeletedAt)
				.HasColumnType("Date")
				.IsRequired(false);

			builder.HasQueryFilter(p => p.DeletedAt == null);
		}
	}
}
