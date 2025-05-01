using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoFactorEmailAuthToLoginAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auth_code",
                schema: "identities",
                table: "login_attempts");

            migrationBuilder.RenameColumn(
                name: "valid_to",
                schema: "identities",
                table: "login_attempts",
                newName: "two_factor_email_authentication_challenge_valid_to");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "two_factor_email_authentication_challenge_valid_to",
                schema: "identities",
                table: "login_attempts",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "auth_code_challenge_created_at",
                schema: "identities",
                table: "login_attempts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "auth_code_challenge_hashed_code",
                schema: "identities",
                table: "login_attempts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "auth_code_challenge_valid_to",
                schema: "identities",
                table: "login_attempts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "two_factor_email_authentication_challenge_code_hash",
                schema: "identities",
                table: "login_attempts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "two_factor_email_authentication_challenge_created_at",
                schema: "identities",
                table: "login_attempts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "two_factor_email_authentication_challenge_session_token_hash",
                schema: "identities",
                table: "login_attempts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "identities",
                table: "login_attempts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "require_mfa",
                schema: "identities",
                table: "identities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auth_code_challenge_created_at",
                schema: "identities",
                table: "login_attempts");

            migrationBuilder.DropColumn(
                name: "auth_code_challenge_hashed_code",
                schema: "identities",
                table: "login_attempts");

            migrationBuilder.DropColumn(
                name: "auth_code_challenge_valid_to",
                schema: "identities",
                table: "login_attempts");

            migrationBuilder.DropColumn(
                name: "two_factor_email_authentication_challenge_code_hash",
                schema: "identities",
                table: "login_attempts");

            migrationBuilder.DropColumn(
                name: "two_factor_email_authentication_challenge_created_at",
                schema: "identities",
                table: "login_attempts");

            migrationBuilder.DropColumn(
                name: "two_factor_email_authentication_challenge_session_token_hash",
                schema: "identities",
                table: "login_attempts");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "identities",
                table: "login_attempts");

            migrationBuilder.DropColumn(
                name: "require_mfa",
                schema: "identities",
                table: "identities");

            migrationBuilder.RenameColumn(
                name: "two_factor_email_authentication_challenge_valid_to",
                schema: "identities",
                table: "login_attempts",
                newName: "valid_to");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "valid_to",
                schema: "identities",
                table: "login_attempts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "auth_code",
                schema: "identities",
                table: "login_attempts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
