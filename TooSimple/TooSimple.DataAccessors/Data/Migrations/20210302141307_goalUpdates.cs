using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.DataAccessors.Data.Migrations
{
    public partial class goalUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "Goal");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountContributed",
                table: "Goal",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountSpent",
                table: "Goal",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "AutoRefill",
                table: "Goal",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountContributed",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "AmountSpent",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "AutoRefill",
                table: "Goal");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBalance",
                table: "Goal",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
