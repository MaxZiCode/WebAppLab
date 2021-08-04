using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace WebAppLab.Data.Migrations
{
	public partial class AddProductRating : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "ProductRating",
				columns: table => new
				{
					ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Rating = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductRating", x => new { x.ProductId, x.UserId });
					table.ForeignKey(
						name: "FK_ProductRating_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ProductRating_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ProductRating_UserId",
				table: "ProductRating",
				column: "UserId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "ProductRating");
		}
	}
}
