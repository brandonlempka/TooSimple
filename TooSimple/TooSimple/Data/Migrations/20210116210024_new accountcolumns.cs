using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.Data.Migrations
{
    public partial class newaccountcolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "Account",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAccountId",
                table: "Account",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "Account");
        }
    }
}
