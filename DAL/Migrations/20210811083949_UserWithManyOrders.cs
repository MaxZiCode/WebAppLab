using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppLab.Data.Migrations
{
	public partial class UserWithManyOrders : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_Orders_UserId",
				table: "Orders");

			migrationBuilder.CreateIndex(
				name: "IX_Orders_UserId",
				table: "Orders",
				column: "UserId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_Orders_UserId",
				table: "Orders");

			migrationBuilder.CreateIndex(
				name: "IX_Orders_UserId",
				table: "Orders",
				column: "UserId",
				unique: true);
		}
	}
}
