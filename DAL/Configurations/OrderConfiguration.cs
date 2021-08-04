using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebAppLab.Data.Models;

namespace WebAppLab.Data.Configurations
{
	public class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.Property(o => o.UserId).IsRequired();

			builder
				.HasOne<User>()
				.WithOne(u => u.Order)
				.HasForeignKey<Order>(o => o.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			builder
				.HasMany(p => p.ProductAmounts)
				.WithOne()
				.HasForeignKey(pa => pa.OrderId);
		}
	}
}
