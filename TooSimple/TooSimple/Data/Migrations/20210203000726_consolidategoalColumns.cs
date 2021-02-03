using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.Data.Migrations
{
    public partial class consolidategoalColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountNeededEachTimeFrame",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "FirstCompletionDate",
                table: "Goal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountNeededEachTimeFrame",
                table: "Goal",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstCompletionDate",
                table: "Goal",
                type: "datetime2",
                nullable: true);
        }
    }
}
