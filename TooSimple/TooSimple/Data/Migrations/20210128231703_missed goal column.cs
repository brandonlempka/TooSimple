using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.Data.Migrations
{
    public partial class missedgoalcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FundingScheduleId",
                table: "Goal",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FundingScheduleId",
                table: "Goal");
        }
    }
}
