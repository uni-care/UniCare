using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPriceOwnedTypeMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentVerifications_UniCare_Users_UserId",
                table: "StudentVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentVerifications",
                table: "StudentVerifications");

            migrationBuilder.RenameTable(
                name: "StudentVerifications",
                newName: "UniCare_StudentVerifications");

            migrationBuilder.RenameIndex(
                name: "IX_StudentVerifications_UserId",
                table: "UniCare_StudentVerifications",
                newName: "IX_UniCare_StudentVerifications_UserId");

            // ── PRICE BACKFILL STARTS HERE ──────────────────────────────────────

            // Step 1: add Currency as nullable first, so existing rows don't violate
            // a NOT NULL constraint before we've populated it
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Items",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            // Step 2: parse the existing "Amount:Currency" string format out of the
            // old Price column into Currency. CHARINDEX finds the ':' separator;
            // everything after it is the currency code.
            migrationBuilder.Sql(@"
        UPDATE Items
        SET Currency = SUBSTRING(Price, CHARINDEX(':', Price) + 1, LEN(Price))
        WHERE Price IS NOT NULL AND CHARINDEX(':', Price) > 0;
 
        UPDATE Items
        SET Currency = 'EGP'
        WHERE Currency IS NULL OR Currency = '';
    ");

            // Step 3: overwrite Price itself, keeping ONLY the numeric portion
            // (everything before the ':'), still as a string for now - the actual
            // type change happens in the next step.
            migrationBuilder.Sql(@"
        UPDATE Items
        SET Price = SUBSTRING(Price, 1, CHARINDEX(':', Price) - 1)
        WHERE Price IS NOT NULL AND CHARINDEX(':', Price) > 0;
    ");

            // Step 4: now that Price only contains numeric strings like '18.00',
            // this conversion is safe.
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Items",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            // Step 5: every row now has a real Currency value from Step 2, so it's
            // safe to enforce NOT NULL.
            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Items",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldNullable: true);

            // ── PRICE BACKFILL ENDS HERE ─────────────────────────────────────────

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "UniCare_StudentVerifications",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 30);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UniCare_StudentVerifications",
                table: "UniCare_StudentVerifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UniCare_StudentVerifications_UniCare_Users_UserId",
                table: "UniCare_StudentVerifications",
                column: "UserId",
                principalTable: "UniCare_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
