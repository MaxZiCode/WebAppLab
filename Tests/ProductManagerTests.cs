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
	public class ProductManagerTests : DbRequiredTests, IAsyncDisposable
	{
		private readonly IImageManager imageManager;
		private readonly IMapper mapper;
		private readonly ApplicationDbContext context;
		private readonly ProductManager productManager;

		public ProductManagerTests()
		{
			imageManager = A.Fake<IImageManager>();

			var configuration = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<ProductViewModel, Product>().ReverseMap();
				cfg.CreateMap<ProductForm, Product>().ReverseMap();
			});
			mapper = new Mapper(configuration);

			context = new ApplicationDbContext(ContextOptions);

			productManager = new ProductManager(context, mapper, imageManager);
		}

		[Fact]
		public async Task GetTopAsync_3_ReturnsPCPS4XboxOneCollection()
		{
			var result = await productManager.GetTopAsync(3);

			var expected = new[] { Platforms.PC, Platforms.PlayStation4, Platforms.XboxOne };
			var actual = result;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task FindByNameAsync_EditionWord_ReturnsMassEffectAndMetroExodus()
		{
			var result = await productManager.FindByNameAsync("Edition");

			var expected = new[] { "Mass Effect Legendary Edition", "Metro Exodus: Complete Edition" }.OrderBy(n => n);
			var actual = result.Select(p => p.Name).OrderBy(n => n);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task FindByIdAsync_MetroExodusId_ReturnsMetroExodusProduct()
		{
			var id = Guid.Parse("446C1DE7-0278-445C-A292-1F3A5A6AE4C3");
			var result = await productManager.FindByIdAsync(id);

			var expected = "Metro Exodus: Complete Edition";
			var actual = result.Name;
			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task CreateAsync_ValidProductForm_ThisProductCanBeFound()
		{
			var arrangeProduct = await context.Products.FirstAsync();
			var arrangeProductForm = GetProductFormAndSetupImageUpload(arrangeProduct);
			arrangeProductForm.Id = Guid.NewGuid();

			var result = await productManager.CreateAsync(arrangeProductForm);

			var dbProducts = await context.Products.FindAsync(result.Id);
			Assert.NotNull(dbProducts);
		}

		[Fact]
		public async Task UpdateAsync_ValidProductFormWithChangedName_NameOfProductHasChangedInResultAndDb()
		{
			var arrangeProduct = await context.Products.FirstAsync();
			var arrangeProductForm = GetProductFormAndSetupImageUpload(arrangeProduct);
			arrangeProductForm.Name = "test name";

			var result = await productManager.UpdateAsync(arrangeProductForm);

			var productsFromDb = await context.Products.FindAsync(result.Id);

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
		public async Task DeleteAsync_ExistingProductId_ProductDeletedAtNotNullAndImagesDeleted()
		{
			var arrangeProduct = await context.Products.FirstAsync();

			await productManager.DeleteAsync(arrangeProduct.Id);

			var assertProduct = await context.Products.FindAsync(arrangeProduct.Id);

			Assert.NotNull(assertProduct.DeletedAt);
			A.CallTo(() => imageManager.DeleteAsync(arrangeProduct.Logo)).MustHaveHappened();
			A.CallTo(() => imageManager.DeleteAsync(arrangeProduct.Background)).MustHaveHappened();
		}

		[Fact]
		public async Task DeleteAsync_InvalidProductId_ContextNotChanged()
		{
			bool changed = false;
			context.ChangeTracker.StateChanged += (o, e) => changed = true;

			await productManager.DeleteAsync(Guid.Empty);

			Assert.False(changed);
		}

		[Fact]
		public async Task AddRatingAndUpdateTotalAsync_NewRatingData_ReturnsThisRatingAndCanBeFoundInDb()
		{
			var product = await context.Products.FirstAsync();
			await ClearRatingsForProductAsync(product.Id);
			string userId = Guid.NewGuid().ToString();
			RatingData rating = new()
			{
				ProductId = product.Id,
				Rating = 70
			};

			var testRating = await productManager.AddRatingAndUpdateTotalAsync(userId, rating);

			var dbRating = await context.ProductRatings.FindAsync(rating.ProductId, userId);

			Assert.Equal(product.Id, testRating.ProductId);
			Assert.Equal(userId, testRating.UserId);

			Assert.Equal(product.Id, dbRating.ProductId);
			Assert.Equal(userId, dbRating.UserId);
		}

		[Fact]
		public async Task AddRatingAndUpdateTotalAsync_TwoRatingDataWithSameProductId_DbHasOnlySecondRating()
		{
			var product = await context.Products.FirstAsync();
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

			await productManager.AddRatingAndUpdateTotalAsync(userId, firstRating);
			await productManager.AddRatingAndUpdateTotalAsync(userId, secondRating);

			var dbRating = await context.ProductRatings.FindAsync(product.Id, userId);

			Assert.Equal(secondRating.Rating, dbRating.Rating);
		}

		[Fact]
		public async Task AddRatingAndUpdateTotalAsync_ThreeRatingDataWithSameProductIdAndDifferentUserID_ProductTotalRatingUpdated()
		{
			var product = await context.Products.FirstAsync();
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

			await productManager.AddRatingAndUpdateTotalAsync(firstUserId, firstRating);
			await productManager.AddRatingAndUpdateTotalAsync(secondUserId, secondRating);
			await productManager.AddRatingAndUpdateTotalAsync(thirdUserId, thirdRating);

			int expected = (firstRating.Rating + secondRating.Rating + thirdRating.Rating) / 3;
			int actual = (await context.Products.FindAsync(product.Id)).TotalRating;
			Assert.Equal(expected, actual);
		}

		private async Task ClearRatingsForProductAsync(Guid productId)
		{
			var product = await context.Products.FindAsync(productId);
			var ratingsForProduct = context.ProductRatings.Where(pr => pr.ProductId == product.Id);
			context.ProductRatings.RemoveRange(ratingsForProduct);
			await context.SaveChangesAsync();
		}

		[Theory]
		[InlineData(1, 1)]
		[InlineData(1, 5)]
		[InlineData(2, 1)]
		[InlineData(2, 5)]
		[InlineData(20, 5)]
		[InlineData(2, 50)]
		public async Task GetPagesAsync_PageNumberPageSizeVary_ReturnsPagesWithDefaultSortedProducts(int pageNumber, int pageSize)
		{
			ProductListData arrangeData = new()
			{
				PageNumber = pageNumber,
				PageSize = pageSize
			};

			var testPages = await productManager.GetPagesAsync(arrangeData);

			var expected = await context.Products.OrderByDescending(o => o.TotalRating).Skip((arrangeData.PageNumber - 1) * arrangeData.PageSize).Take(arrangeData.PageSize).Select(p => p.Id).ToListAsync();
			var actual = testPages.Data.Select(p => p.Id);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("shooter", null)]
		[InlineData(null, Rating.PEGI18)]
		[InlineData("shooter", Rating.PEGI18)]
		public async Task GetPagesAsync_GenreRatingVary_ReturnsPagesWithFilteredProducts(string genre, Rating? age)
		{
			var products = await context.Products.ToListAsync();
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
			await context.SaveChangesAsync();
			ProductListData assertData = new()
			{
				PageNumber = 1,
				PageSize = 5,
				Genre = genre,
				Age = age
			};

			var testPages = await productManager.GetPagesAsync(assertData);

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
		public async Task GetPagesAsync_SortBySortDirectionVary_ReturnsPagesWithSortedProducts(SortBy? sort, bool isSortAsc)
		{
			var products = await context.Products.ToListAsync();
			products.ForEach(p => SetPriceAndTotalRating(p, isSortAsc ? 100 : 0));
			SetPriceAndTotalRating(products[0], isSortAsc ? 49 : 51);
			SetPriceAndTotalRating(products[1], 50);
			SetPriceAndTotalRating(products[2], isSortAsc ? 51 : 49);
			await context.SaveChangesAsync();
			ProductListData assertData = new()
			{
				PageNumber = 1,
				PageSize = 5,
				SortBy = sort,
				IsSortAsc = isSortAsc
			};

			var testPages = await productManager.GetPagesAsync(assertData);

			var expected = products.Take(3).Select(p => p.Id);
			var actual = testPages.Data.Take(3).Select(p => p.Id);
			Assert.Equal(expected, actual);
		}

		private void SetPriceAndTotalRating(Product product, int value)
		{
			product.Price = value;
			product.TotalRating = value;
		}

		public ValueTask DisposeAsync()
		{
			return ((IAsyncDisposable)context).DisposeAsync();
		}
	}
}
