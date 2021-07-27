using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace WebAppLab.Data.Migrations
{
	public partial class Products : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Products",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Platform = table.Column<int>(type: "int", nullable: false),
					DateCreated = table.Column<DateTime>(type: "Date", nullable: false),
					TotalRating = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Products", x => x.Id);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Products_Name_Platform_DateCreated_TotalRating",
				table: "Products",
				columns: new[] { "Name", "Platform", "DateCreated", "TotalRating" });
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Products");
		}
	}
}
