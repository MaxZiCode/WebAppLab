using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace WebAppLab.Data.Migrations
{
	public partial class UserDbo : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "AspNetUserTokens",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "LoginProvider",
				table: "AspNetUserTokens",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AddColumn<string>(
				name: "AddressDelivery",
				table: "AspNetUsers",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<DateTime>(
				name: "DateOfBirth",
				table: "AspNetUsers",
				type: "datetime2",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<string>(
				name: "FirstName",
				table: "AspNetUsers",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "LastName",
				table: "AspNetUsers",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ProviderKey",
				table: "AspNetUserLogins",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "LoginProvider",
				table: "AspNetUserLogins",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AddressDelivery",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "DateOfBirth",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "FirstName",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "LastName",
				table: "AspNetUsers");


			migrationBuilder.DropPrimaryKey("PK_AspNetUserTokens", "AspNetUserTokens");
			migrationBuilder.DropPrimaryKey("PK_AspNetUserLogins", "AspNetUserLogins");


			migrationBuilder.AlterColumn<string>(
				name: "Name",
				table: "AspNetUserTokens",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "LoginProvider",
				table: "AspNetUserTokens",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "ProviderKey",
				table: "AspNetUserLogins",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "LoginProvider",
				table: "AspNetUserLogins",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AddPrimaryKey("PK_AspNetUserTokens", "AspNetUserTokens", new string[] { "UserId", "LoginProvider", "Name" });
			migrationBuilder.AddPrimaryKey("PK_AspNetUserLogins", "AspNetUserLogins", new string[] { "LoginProvider", "ProviderKey" });
		}
	}
}
