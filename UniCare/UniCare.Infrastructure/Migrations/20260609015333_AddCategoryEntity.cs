using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryEntity : Migration
    {
        /// <inheritdoc />
        private static readonly Guid OtherCategoryId = new("11111111-1111-1111-1111-111111111111");
        private static readonly Guid TextbooksCategoryId = new("22222222-2222-2222-2222-222222222222");
        private static readonly Guid LabScienceCategoryId = new("33333333-3333-3333-3333-333333333333");
        private static readonly Guid ArtDesignCategoryId = new("44444444-4444-4444-4444-444444444444");
        private static readonly Guid EngineeringTechCategoryId = new("55555555-5555-5555-5555-555555555555");
        private static readonly Guid MedicalHealthCategoryId = new("66666666-6666-6666-6666-666666666666");
        private static readonly Guid ElectronicsCategoryId = new("77777777-7777-7777-7777-777777777777");
        private static readonly Guid MusicPerformingArtsCategoryId = new("88888888-8888-8888-8888-888888888888");
        private static readonly Guid SportsRecreationCategoryId = new("99999999-9999-9999-9999-999999999999");
        private static readonly Guid DormLivingCategoryId = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var seedDate = new DateTime(2026, 6, 9, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "Description", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { OtherCategoryId, "Other", "Used supplies that do not fit other categories", seedDate, seedDate },
                    { TextbooksCategoryId, "Textbooks & Course Materials", "Textbooks, readers, study guides, and course packs", seedDate, seedDate },
                    { LabScienceCategoryId, "Lab & Science Supplies", "Lab coats, goggles, glassware, models, and science kits", seedDate, seedDate },
                    { ArtDesignCategoryId, "Art & Design Supplies", "Sketchbooks, paints, brushes, drafting tools, and portfolios", seedDate, seedDate },
                    { EngineeringTechCategoryId, "Engineering & Tech Tools", "Calculators, multimeters, toolkits, and technical equipment", seedDate, seedDate },
                    { MedicalHealthCategoryId, "Medical & Health Sciences", "Scrubs, stethoscopes, anatomy models, and clinical supplies", seedDate, seedDate },
                    { ElectronicsCategoryId, "Electronics & Devices", "Laptops, tablets, headphones, cameras, and chargers", seedDate, seedDate },
                    { MusicPerformingArtsCategoryId, "Music & Performing Arts", "Instruments, sheet music, stands, and stage equipment", seedDate, seedDate },
                    { SportsRecreationCategoryId, "Sports & Recreation", "Team gear, gym equipment, bikes, and outdoor gear", seedDate, seedDate },
                    { DormLivingCategoryId, "Dorm & Living Essentials", "Furniture, kitchenware, bedding, and storage for campus living", seedDate, seedDate }
                });

            migrationBuilder.Sql($"""
                UPDATE Items
                SET CategoryId = '{OtherCategoryId}'
                WHERE CategoryId = '00000000-0000-0000-0000-000000000000'
                   OR CategoryId NOT IN (
                       SELECT Id FROM Categories
                   )
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryId",
                table: "Items");
        }
    }
}
