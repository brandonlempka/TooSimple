using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.DataAccessors.Data.Migrations
{
    public partial class tokenupdatecolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelogToken",
                table: "Account",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelogToken",
                table: "Account");
        }
    }
}
