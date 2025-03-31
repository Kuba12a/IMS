using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionsAndLoginAttemptsToIdentityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "login_attempts",
                schema: "identities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    auth_code = table.Column<string>(type: "text", nullable: false),
                    code_challenge = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    identity_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_login_attempts_identities_identity_id",
                        column: x => x.identity_id,
                        principalSchema: "identities",
                        principalTable: "identities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                schema: "identities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    refresh_token_hash = table.Column<string>(type: "text", nullable: false),
                    ip_address = table.Column<string>(type: "text", nullable: false),
                    identity_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_sessions_identities_identity_id",
                        column: x => x.identity_id,
                        principalSchema: "identities",
                        principalTable: "identities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_login_attempts_identity_id",
                schema: "identities",
                table: "login_attempts",
                column: "identity_id");

            migrationBuilder.CreateIndex(
                name: "ix_sessions_identity_id",
                schema: "identities",
                table: "sessions",
                column: "identity_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "login_attempts",
                schema: "identities");

            migrationBuilder.DropTable(
                name: "sessions",
                schema: "identities");
        }
    }
}
