using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppLab.Data.Migrations
{
	public partial class ProductPriceDecimal : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<decimal>(
				name: "Price",
				table: "Products",
				type: "DECIMAL(16,4)",
				nullable: false,
				oldClrType: typeof(decimal),
				oldType: "Money");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<decimal>(
				name: "Price",
				table: "Products",
				type: "Money",
				nullable: false,
				oldClrType: typeof(decimal),
				oldType: "DECIMAL(16,4)");
		}
	}
}
