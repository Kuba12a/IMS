using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identities");

            migrationBuilder.CreateTable(
                name: "identities",
                schema: "identities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    password_reset_token_hash = table.Column<string>(type: "text", nullable: true),
                    password_reset_token_valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    email_confirmation_token_hash = table.Column<string>(type: "text", nullable: true),
                    email_confirmation_token_valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identities", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_identities_email",
                schema: "identities",
                table: "identities",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identities_email_confirmation_token_hash",
                schema: "identities",
                table: "identities",
                column: "email_confirmation_token_hash");

            migrationBuilder.CreateIndex(
                name: "ix_identities_password_reset_token_hash",
                schema: "identities",
                table: "identities",
                column: "password_reset_token_hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identities",
                schema: "identities");
        }
    }
}
