using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebAppLab.Data.Models;

namespace WebAppLab.Data.Configurations
{
	public class ProductAmountConfiguration : IEntityTypeConfiguration<ProductAmount>
	{
		public void Configure(EntityTypeBuilder<ProductAmount> builder)
		{
			builder.HasKey(pa => new { pa.OrderId, pa.ProductId });

			builder.Property(pa => pa.Amount).IsRequired().HasDefaultValue(1);

			builder
				.HasOne<Order>()
				.WithMany(o => o.ProductAmounts)
				.HasForeignKey(o => o.OrderId)
				.OnDelete(DeleteBehavior.Cascade);
			builder
				.HasOne<Product>()
				.WithMany()
				.HasForeignKey(o => o.ProductId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
