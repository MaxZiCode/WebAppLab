using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace WebAppLab.Data.Migrations
{
	public partial class AddProductAmounts : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Products_Orders_OrderId",
				table: "Products");

			migrationBuilder.DropIndex(
				name: "IX_Products_OrderId",
				table: "Products");

			migrationBuilder.DropColumn(
				name: "OrderId",
				table: "Products");

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "Orders",
				type: "nvarchar(450)",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "nvarchar(max)",
				oldNullable: true);

			migrationBuilder.CreateTable(
				name: "ProductAmounts",
				columns: table => new
				{
					OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Amount = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductAmounts", x => new { x.OrderId, x.ProductId });
					table.ForeignKey(
						name: "FK_ProductAmounts_Orders_OrderId",
						column: x => x.OrderId,
						principalTable: "Orders",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ProductAmounts_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Orders_UserId",
				table: "Orders",
				column: "UserId",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_ProductAmounts_ProductId",
				table: "ProductAmounts",
				column: "ProductId");

			migrationBuilder.AddForeignKey(
				name: "FK_Orders_AspNetUsers_UserId",
				table: "Orders",
				column: "UserId",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Orders_AspNetUsers_UserId",
				table: "Orders");

			migrationBuilder.DropTable(
				name: "ProductAmounts");

			migrationBuilder.DropIndex(
				name: "IX_Orders_UserId",
				table: "Orders");

			migrationBuilder.AddColumn<Guid>(
				name: "OrderId",
				table: "Products",
				type: "uniqueidentifier",
				nullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "Orders",
				type: "nvarchar(max)",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.CreateIndex(
				name: "IX_Products_OrderId",
				table: "Products",
				column: "OrderId");

			migrationBuilder.AddForeignKey(
				name: "FK_Products_Orders_OrderId",
				table: "Products",
				column: "OrderId",
				principalTable: "Orders",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}
