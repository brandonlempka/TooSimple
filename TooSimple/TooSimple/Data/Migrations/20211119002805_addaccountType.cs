using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.Data.Migrations
{
    public partial class addaccountType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelogToken",
                table: "Account");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelogToken",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
