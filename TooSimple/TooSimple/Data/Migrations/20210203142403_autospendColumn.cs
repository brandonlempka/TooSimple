using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.Data.Migrations
{
    public partial class autospendColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutoSpendMerchantName",
                table: "Goal",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoSpendMerchantName",
                table: "Goal");
        }
    }
}
