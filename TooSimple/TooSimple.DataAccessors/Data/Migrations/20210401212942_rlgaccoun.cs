using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.DataAccessors.Data.Migrations
{
    public partial class rlgaccoun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReLoginRequired",
                table: "Account",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReLoginRequired",
                table: "Account");
        }
    }
}
