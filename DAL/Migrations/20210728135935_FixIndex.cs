using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppLab.Data.Migrations
{
	public partial class FixIndex : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_Products_Name_Platform_DateCreated_TotalRating",
				table: "Products");

			migrationBuilder.CreateIndex(
				name: "IX_Products_DateCreated",
				table: "Products",
				column: "DateCreated");

			migrationBuilder.CreateIndex(
				name: "IX_Products_Name",
				table: "Products",
				column: "Name");

			migrationBuilder.CreateIndex(
				name: "IX_Products_Platform",
				table: "Products",
				column: "Platform");

			migrationBuilder.CreateIndex(
				name: "IX_Products_TotalRating",
				table: "Products",
				column: "TotalRating");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_Products_DateCreated",
				table: "Products");

			migrationBuilder.DropIndex(
				name: "IX_Products_Name",
				table: "Products");

			migrationBuilder.DropIndex(
				name: "IX_Products_Platform",
				table: "Products");

			migrationBuilder.DropIndex(
				name: "IX_Products_TotalRating",
				table: "Products");

			migrationBuilder.CreateIndex(
				name: "IX_Products_Name_Platform_DateCreated_TotalRating",
				table: "Products",
				columns: new[] { "Name", "Platform", "DateCreated", "TotalRating" });
		}
	}
}
