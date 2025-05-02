using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "identities",
                table: "identities",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                schema: "identities",
                table: "identities");
        }
    }
}
