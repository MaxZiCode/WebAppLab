using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace WebAppLab.Data.Migrations
{
	public partial class AddProductsData : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.InsertData(
				table: "Products",
				columns: new[] { "Id", "DateCreated", "Name", "Platform", "TotalRating" },
				values: new object[,]
				{
					{ new Guid("90faa83b-0427-44b5-8d35-5f81bbbad1c4"), new DateTime(2021, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Biomutant", 1, 64 },
					{ new Guid("cdc22ab5-abf1-4c20-8bd6-3220c0c4d7a6"), new DateTime(2021, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Astalon: Tears of the Earth", 0, 87 },
					{ new Guid("752b8b53-c787-4a8e-84f8-a74fafc04e96"), new DateTime(2021, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lacuna - A Sci-Fi Noir Adventure", 0, 88 },
					{ new Guid("c29337a4-cebb-47fd-b9a3-3f60e16468a0"), new DateTime(2021, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wildermyth", 0, 89 },
					{ new Guid("36e9cea7-27f6-4f85-a078-156e7817a75a"), new DateTime(2021, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chicory: A Colorful Tale", 0, 89 },
					{ new Guid("19f1fa2f-bba6-4d87-a727-6187e02b2644"), new DateTime(2020, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Persona 5 Royal", 1, 95 },
					{ new Guid("64d67d60-9b65-415b-b826-942bdfb01605"), new DateTime(2018, 10, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Red Dead Redemption 2", 1, 97 },
					{ new Guid("659d2773-00f2-4d23-9208-7dab6f7eccfa"), new DateTime(2021, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "ScourgeBringer", 1, 81 },
					{ new Guid("cf64f991-31f1-4fa2-8db6-f274bc8640d5"), new DateTime(2021, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Guilty Gear -Strive-", 1, 83 },
					{ new Guid("c99a81f9-2056-41a3-a968-63238637b3f3"), new DateTime(2021, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Green Hell", 2, 85 },
					{ new Guid("44e2e106-5a13-4863-8326-a01e340883e4"), new DateTime(2021, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Final Fantasy VII Remake Intergrade", 4, 89 },
					{ new Guid("cf6b66bc-a9d2-4aec-a9c6-e8322c079e03"), new DateTime(2021, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mass Effect Legendary Edition", 2, 90 },
					{ new Guid("0260ff54-243e-4cf1-8332-77976d00c3bc"), new DateTime(2021, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dungeons & Dragons: Dark Alliance", 4, 58 },
					{ new Guid("237c3ba6-5fdc-4644-ad9b-1ab8a629edfa"), new DateTime(2021, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Scarlet Nexus", 5, 77 },
					{ new Guid("f1b8089d-5dd7-4c37-8053-ddec627ec63d"), new DateTime(2021, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Monster Hunter Stories 2: Wings of Ruin", 3, 81 },
					{ new Guid("1663cfef-1671-4216-839d-8dc119b27bf2"), new DateTime(2021, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "NieR Replicant ver.1.22474487139", 2, 83 },
					{ new Guid("19dd47b4-8f29-4a27-8b3c-113d799c7b08"), new DateTime(2021, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "F1 2021", 0, 86 },
					{ new Guid("446c1de7-0278-445c-a292-1f3a5a6ae4c3"), new DateTime(2021, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Metro Exodus: Complete Edition", 0, 85 }
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("0260ff54-243e-4cf1-8332-77976d00c3bc"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("1663cfef-1671-4216-839d-8dc119b27bf2"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("19dd47b4-8f29-4a27-8b3c-113d799c7b08"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("19f1fa2f-bba6-4d87-a727-6187e02b2644"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("237c3ba6-5fdc-4644-ad9b-1ab8a629edfa"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("36e9cea7-27f6-4f85-a078-156e7817a75a"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("446c1de7-0278-445c-a292-1f3a5a6ae4c3"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("44e2e106-5a13-4863-8326-a01e340883e4"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("64d67d60-9b65-415b-b826-942bdfb01605"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("659d2773-00f2-4d23-9208-7dab6f7eccfa"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("752b8b53-c787-4a8e-84f8-a74fafc04e96"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("90faa83b-0427-44b5-8d35-5f81bbbad1c4"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("c29337a4-cebb-47fd-b9a3-3f60e16468a0"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("c99a81f9-2056-41a3-a968-63238637b3f3"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("cdc22ab5-abf1-4c20-8bd6-3220c0c4d7a6"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("cf64f991-31f1-4fa2-8db6-f274bc8640d5"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("cf6b66bc-a9d2-4aec-a9c6-e8322c079e03"));

			migrationBuilder.DeleteData(
				table: "Products",
				keyColumn: "Id",
				keyValue: new Guid("f1b8089d-5dd7-4c37-8053-ddec627ec63d"));
		}
	}
}
