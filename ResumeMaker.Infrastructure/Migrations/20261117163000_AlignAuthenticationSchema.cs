using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMaker.Infrastructure.Migrations;

public partial class AlignAuthenticationSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "RefreshTokens",
            type: "datetime2",
            nullable: false,
            defaultValueSql: "GETUTCDATE()");

        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(256)",
            maxLength: 256,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PasswordHash",
            table: "Users",
            type: "nvarchar(256)",
            maxLength: 256,
            nullable: true);

        migrationBuilder.AlterColumn<long>(
            name: "TelegramUserId",
            table: "Users",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AlterColumn<string>(
            name: "TelegramUsername",
            table: "TelegramAuthRequests",
            type: "nvarchar(64)",
            maxLength: 64,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true,
            filter: "[Email] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");

        migrationBuilder.DropColumn(name: "CreatedAt", table: "RefreshTokens");
        migrationBuilder.DropColumn(name: "Email", table: "Users");
        migrationBuilder.DropColumn(name: "PasswordHash", table: "Users");

        migrationBuilder.AlterColumn<long>(
            name: "TelegramUserId",
            table: "Users",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "TelegramUsername",
            table: "TelegramAuthRequests",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(64)",
            oldMaxLength: 64,
            oldNullable: true);
    }
}
