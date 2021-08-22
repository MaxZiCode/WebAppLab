using AutoMapper;

using FakeItEasy;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAppLab.Data;
using WebAppLab.Data.Constants;
using WebAppLab.Data.Models;
using WebAppLab.Models;
using WebAppLab.Services;

using Xunit;

namespace WebAppLab.Tests
{
	public class ProductManagerTests : DbRequiredTests
	{
		private readonly IImageManager imageManager;
		private readonly IMapper mapper;

		public ProductManagerTests()
		{
			imageManager = A.Fake<IImageManager>();

			var configuration = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<ProductViewModel, Product>().ReverseMap();
				cfg.CreateMap<ProductForm, Product>().ReverseMap();
			});
			mapper = new Mapper(configuration);
		}

		[Fact]
		public async Task Can_get_top3_Platforms()
		{
			var result = await TestProductManagerMethod(pm => pm.GetTopAsync(3));

			var expected = new[] { Platforms.PC, Platforms.PlayStation4, Platforms.XboxOne };
			var actual = result;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Can_find_by_name_contains_edition()
		{
			var result = await TestProductManagerMethod(pm => pm.FindByNameAsync("Edition"));

			var expected = new[] { "Mass Effect Legendary Edition", "Metro Exodus: Complete Edition" }.OrderBy(n => n);
			var actual = result.Select(p => p.Name).OrderBy(n => n);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Can_find_by_id()
		{
			var id = Guid.Parse("446C1DE7-0278-445C-A292-1F3A5A6AE4C3");
			var result = await TestProductManagerMethod(pm => pm.FindByIdAsync(id));

			var expected = "Metro Exodus: Complete Edition";
			var actual = result.Name;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Can_create()
		{
			var arrangeProduct = await GetFirstProductAsync();
			var arrangeProductForm = GetProductFormAndSetupImageUpload(arrangeProduct);
			arrangeProductForm.Id = Guid.NewGuid();

			var result = await TestProductManagerMethod(pm => pm.CreateAsync(arrangeProductForm));

			using var arrangeContext = new ApplicationDbContext(ContextOptions);
			var dbProducts = await arrangeContext.Products.FindAsync(result.Id);
			Assert.NotNull(dbProducts);

		}

		[Fact]
		public async Task Can_update()
		{
			var arrangeProduct = await GetFirstProductAsync();
			var arrangeProductForm = GetProductFormAndSetupImageUpload(arrangeProduct);
			arrangeProductForm.Name = "test name";

			var result = await TestProductManagerMethod(pm => pm.UpdateAsync(arrangeProductForm));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var productsFromDb = await assertContext.Products.FindAsync(result.Id);

			var expected = arrangeProductForm.Name;
			Assert.Equal(expected, productsFromDb.Name);
			Assert.Equal(expected, result.Name);
		}

		private ProductForm GetProductFormAndSetupImageUpload(Product product)
		{
			var arrangeProductForm = mapper.Map<ProductForm>(product);
			arrangeProductForm.Logo = A.Fake<IFormFile>();
			arrangeProductForm.Background = null;

			A.CallTo(() => imageManager.UploadAsync(arrangeProductForm.Logo)).Returns(product.Logo);
			A.CallTo(() => imageManager.UploadAsync(arrangeProductForm.Background)).Returns(product.Background);
			return arrangeProductForm;
		}

		[Fact]
		public async Task Can_delete_product_and_images()
		{
			var arrangeProduct = await GetFirstProductAsync();

			await TestProductManagerMethod(pm => pm.DeleteAsync(arrangeProduct.Id));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var assertProduct = await assertContext.Products.FindAsync(arrangeProduct.Id);

			Assert.Null(assertProduct);
			A.CallTo(() => imageManager.DeleteAsync(arrangeProduct.Logo)).MustHaveHappened();
			A.CallTo(() => imageManager.DeleteAsync(arrangeProduct.Background)).MustHaveHappened();
		}

		[Fact]
		public async Task Cannot_delete_product_not_found()
		{
			using var arrangeContext = new ApplicationDbContext(ContextOptions);
			var countBefore = await arrangeContext.Products.CountAsync();

			await TestProductManagerMethod(pm => pm.DeleteAsync(Guid.Empty));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var countAfter = await arrangeContext.Products.CountAsync();

			Assert.Equal(countBefore, countAfter);
		}

		[Fact]
		public async Task Can_add_rating()
		{
			var product = await GetFirstProductAsync();
			await ClearRatingsForProductAsync(product.Id);
			string userId = Guid.NewGuid().ToString();
			RatingData rating = new()
			{
				ProductId = product.Id,
				Rating = 70
			};

			var testRating = await TestProductManagerMethod(pm => pm.AddRatingAndUpdateTotalAsync(userId, rating));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var dbRating = await assertContext.ProductRatings.FindAsync(rating.ProductId, userId);

			Assert.Equal(product.Id, testRating.ProductId);
			Assert.Equal(userId, testRating.UserId);

			Assert.Equal(product.Id, dbRating.ProductId);
			Assert.Equal(userId, dbRating.UserId);
		}

		[Fact]
		public async Task Can_add_rating_overwrite_existing()
		{
			var product = await GetFirstProductAsync();
			string userId = Guid.NewGuid().ToString();
			RatingData firstRating = new()
			{
				ProductId = product.Id,
				Rating = 70
			};
			RatingData secondRating = new()
			{
				ProductId = product.Id,
				Rating = 80
			};

			await TestProductManagerMethod(pm => pm.AddRatingAndUpdateTotalAsync(userId, firstRating));
			var testRating = await TestProductManagerMethod(pm => pm.AddRatingAndUpdateTotalAsync(userId, secondRating));

			Assert.Equal(secondRating.Rating, testRating.Rating);
		}

		[Fact]
		public async Task Adding_rating_updates_total_rating()
		{
			var product = await GetFirstProductAsync();
			await ClearRatingsForProductAsync(product.Id);
			var firstUserId = Guid.NewGuid().ToString();
			RatingData firstRating = new()
			{
				ProductId = product.Id,
				Rating = 10
			};
			var secondUserId = Guid.NewGuid().ToString();
			RatingData secondRating = new()
			{
				ProductId = product.Id,
				Rating = firstRating.Rating + 15
			};
			var thirdUserId = Guid.NewGuid().ToString();
			RatingData thirdRating = new()
			{
				ProductId = product.Id,
				Rating = secondRating.Rating + 20
			};

			await TestProductManagerMethod(pm => pm.AddRatingAndUpdateTotalAsync(firstUserId, firstRating));
			await TestProductManagerMethod(pm => pm.AddRatingAndUpdateTotalAsync(secondUserId, secondRating));
			await TestProductManagerMethod(pm => pm.AddRatingAndUpdateTotalAsync(thirdUserId, thirdRating));

			using var assertContext = new ApplicationDbContext(ContextOptions);
			var dbProduct = await assertContext.Products.FindAsync(firstRating.ProductId);
			int expected = (firstRating.Rating + secondRating.Rating + thirdRating.Rating) / 3;
			int actual = dbProduct.TotalRating;
			Assert.Equal(expected, actual);
		}

		private async Task ClearRatingsForProductAsync(Guid productId)
		{
			using var context = new ApplicationDbContext(ContextOptions);
			var product = await context.Products.FindAsync(productId);
			var ratingsForProduct = context.ProductRatings.Where(pr => pr.ProductId == product.Id);
			context.ProductRatings.RemoveRange(ratingsForProduct);
			await context.SaveChangesAsync();
		}

		private async Task<Product> GetFirstProductAsync()
		{
			using var arrangeContext = new ApplicationDbContext(ContextOptions);
			return await arrangeContext.Products.FirstAsync();
		}

		[Theory]
		[InlineData(1, 1)]
		[InlineData(1, 5)]
		[InlineData(2, 1)]
		[InlineData(2, 5)]
		[InlineData(20, 5)]
		[InlineData(2, 50)]
		public async Task Can_get_pages(int pageNumber, int pageSize)
		{
			ProductListData arrangeData = new()
			{
				PageNumber = pageNumber,
				PageSize = pageSize
			};

			var testPages = await TestProductManagerMethod(pm => pm.GetPagesAsync(arrangeData));

			using var context = new ApplicationDbContext(ContextOptions);
			var expected = await context.Products.OrderByDescending(o => o.TotalRating).Skip((arrangeData.PageNumber - 1) * arrangeData.PageSize).Take(arrangeData.PageSize).Select(p => p.Id).ToListAsync();
			var actual = testPages.Data.Select(p => p.Id);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("shooter", null)]
		[InlineData(null, Rating.PEGI18)]
		[InlineData("shooter", Rating.PEGI18)]
		public async Task Can_get_pages_filtered(string genre, Rating? age)
		{
			using var arrangeContext = new ApplicationDbContext(ContextOptions);
			var products = await arrangeContext.Products.ToListAsync();
			products.ForEach(p =>
			{
				p.Genre = "notEmptyGenre";
				p.Rating = Rating.PEGI3;
			});

			HashSet<Guid> arrangeIds = new();
			if (genre != null)
			{
				products[0].Genre = genre;
				products[1].Genre = genre;
				arrangeIds.Add(products[0].Id);
				arrangeIds.Add(products[1].Id);
			}
			else if (age != null)
			{
				products[1].Rating = age.Value;
				products[2].Rating = age.Value;
				arrangeIds.Add(products[1].Id);
				arrangeIds.Add(products[2].Id);
			}
			await arrangeContext.SaveChangesAsync();
			ProductListData assertData = new()
			{
				PageNumber = 1,
				PageSize = 5,
				Genre = genre,
				Age = age
			};

			var testPages = await TestProductManagerMethod(pm => pm.GetPagesAsync(assertData));

			var expected = arrangeIds.OrderBy(id => id);
			var actual = testPages.Data.Select(p => p.Id).OrderBy(id => id);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(null, false)]
		[InlineData(SortBy.Price, false)]
		[InlineData(SortBy.TotalRating, false)]
		[InlineData(SortBy.Price, true)]
		[InlineData(SortBy.TotalRating, true)]
		public async Task Can_get_pages_sorted(SortBy? sort, bool isSortAsc)
		{
			using var arrangeContext = new ApplicationDbContext(ContextOptions);
			var products = await arrangeContext.Products.ToListAsync();
			products.ForEach(p => SetPriceAndTotalRating(p, isSortAsc ? 100 : 0));
			SetPriceAndTotalRating(products[0], isSortAsc ? 49 : 51);
			SetPriceAndTotalRating(products[1], 50);
			SetPriceAndTotalRating(products[2], isSortAsc ? 51 : 49);
			await arrangeContext.SaveChangesAsync();
			ProductListData assertData = new()
			{
				PageNumber = 1,
				PageSize = 5,
				SortBy = sort,
				IsSortAsc = isSortAsc
			};

			var testPages = await TestProductManagerMethod(pm => pm.GetPagesAsync(assertData));

			var expected = products.Take(3).Select(p => p.Id);
			var actual = testPages.Data.Take(3).Select(p => p.Id);
			Assert.Equal(expected, actual);
		}

		private void SetPriceAndTotalRating(Product product, int value)
		{
			product.Price = value;
			product.TotalRating = value;
		}

		private async Task TestProductManagerMethod(Func<ProductManager, Task> methodPicker)
		{
			using var context = new ApplicationDbContext(ContextOptions);
			ProductManager ProductManager = new(context, mapper, imageManager);
			await methodPicker(ProductManager);
		}

		private async Task<T> TestProductManagerMethod<T>(Func<ProductManager, Task<T>> methodPicker)
		{
			using var context = new ApplicationDbContext(ContextOptions);
			ProductManager ProductManager = new(context, mapper, imageManager);
			return await methodPicker(ProductManager);
		}
	}
}
