using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicTestSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_IdentityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "identity_id",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "identity_id",
                table: "users");
        }
    }
}
