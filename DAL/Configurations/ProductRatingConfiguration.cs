using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WebAppLab.Data.Models;

namespace WebAppLab.Data.Configurations
{
	public class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
	{
		public void Configure(EntityTypeBuilder<ProductRating> builder)
		{
			builder.HasKey(pr => new { pr.ProductId, pr.UserId });

			builder.Property(pr => pr.Rating)
				.IsRequired();

			builder.HasOne<Product>().WithMany(p => p.ProductRatings).HasForeignKey(pr => pr.ProductId);
			builder.HasOne<User>().WithMany(p => p.ProductRatings).HasForeignKey(pr => pr.UserId);
		}
	}
}
