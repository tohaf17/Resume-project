using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMaker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramUsernameToAuthRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelegramUsername",
                table: "TelegramAuthRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramUsername",
                table: "TelegramAuthRequests");
        }
    }
}
