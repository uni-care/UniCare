using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Items",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Items",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_Items_Name",
                table: "Items",
                newName: "IX_Items_Title");

            migrationBuilder.AlterColumn<string>(
                name: "VerificationStatus",
                table: "UniCare_Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationMethod",
                table: "UniCare_Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "Items",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Items",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableTo",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Items",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_UniCare_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "UniCare_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedAt",
                table: "Items",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Status",
                table: "Items",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_ItemId",
                table: "UserFavorites",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId_ItemId",
                table: "UserFavorites",
                columns: new[] { "UserId", "ItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropIndex(
                name: "IX_Items_CreatedAt",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Status",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AvailableTo",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Items",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Items",
                newName: "Quantity");

            migrationBuilder.RenameIndex(
                name: "IX_Items_Title",
                table: "Items",
                newName: "IX_Items_Name");

            migrationBuilder.AlterColumn<int>(
                name: "VerificationStatus",
                table: "UniCare_Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "RegistrationMethod",
                table: "UniCare_Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Items",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Items",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
