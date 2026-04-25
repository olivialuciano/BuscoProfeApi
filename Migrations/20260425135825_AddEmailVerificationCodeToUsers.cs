using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuscoProfe.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationCodeToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailConfirmationTokenExpiresAt",
                table: "Users",
                newName: "EmailVerificationCodeLastSentAt");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmationToken",
                table: "Users",
                newName: "EmailVerificationCode");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationCodeExpiresAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationCodeExpiresAt",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "EmailVerificationCodeLastSentAt",
                table: "Users",
                newName: "EmailConfirmationTokenExpiresAt");

            migrationBuilder.RenameColumn(
                name: "EmailVerificationCode",
                table: "Users",
                newName: "EmailConfirmationToken");
        }
    }
}
