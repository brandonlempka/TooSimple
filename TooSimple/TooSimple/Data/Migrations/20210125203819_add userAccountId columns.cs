using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.Data.Migrations
{
    public partial class adduserAccountIdcolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAccountId",
                table: "Goal",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAccountId",
                table: "FundingSchedule",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAccountId",
                table: "Expense",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "FundingSchedule");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "Expense");
        }
    }
}
