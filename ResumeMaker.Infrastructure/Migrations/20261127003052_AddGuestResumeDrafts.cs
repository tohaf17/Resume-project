using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMaker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestResumeDrafts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "ResumeTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuestResumeDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuestSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestResumeDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestResumeDrafts_ResumeTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ResumeTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users",
                column: "TelegramUserId",
                unique: true,
                filter: "[TelegramUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GuestResumeDrafts_GuestSessionId",
                table: "GuestResumeDrafts",
                column: "GuestSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestResumeDrafts_TemplateId",
                table: "GuestResumeDrafts",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestResumeDrafts");

            migrationBuilder.DropTable(
                name: "ResumeTemplates");

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
