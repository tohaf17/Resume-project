using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResumeMaker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicApiEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "RegistrationPromptEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SourcePage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TemplateSlug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AnonymousSessionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationPromptEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResumeTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PreviewImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StructureSchemaJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticPages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ResumeTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "IsPremium", "Name", "PreviewImageUrl", "Slug", "StructureSchemaJson" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "IT & Tech", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Чистий одноколонковий шаблон, що ідеально проходить ATS-системи.", true, false, "Мінімалістичний класичний", "/images/templates/minimalist-classic.png", "minimalist-classic", "{\"columns\":1,\"theme\":\"light\"}" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Design & Creative", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Акцентний шаблон із бічною панеллю для навичок та контактів.", true, true, "Сучасний двоколонковий", "/images/templates/modern-two-column.png", "modern-two-column", "{\"columns\":2,\"theme\":\"accent-blue\"}" }
                });

            migrationBuilder.InsertData(
                table: "StaticPages",
                columns: new[] { "Id", "Content", "IsPublished", "Slug", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Resume Maker — сучасний онлайн-конструктор для швидкого створення професійних резюме.", true, "about", "Про сервіс Resume Maker", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Ми поважаємо вашу приватність та захищаємо персональні дані відповідно до чинного законодавства.", true, "privacy-policy", "Політика конфіденційності", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Використовуючи платформу Resume Maker, ви погоджуєтесь із правилами надання сервісу.", true, "terms-of-service", "Умови використання", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users",
                column: "TelegramUserId",
                unique: true,
                filter: "[TelegramUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPromptEvents_CreatedAt",
                table: "RegistrationPromptEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTemplates_Slug",
                table: "ResumeTemplates",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaticPages_Slug",
                table: "StaticPages",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationPromptEvents");

            migrationBuilder.DropTable(
                name: "ResumeTemplates");

            migrationBuilder.DropTable(
                name: "StaticPages");

            migrationBuilder.DropIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users",
                column: "TelegramUserId",
                unique: true);
        }
    }
}
