using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.DataAccessors.Data.Migrations
{
    public partial class addexpensestogoaltable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountNeededEachTimeFrame",
                table: "Goal",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "ExpenseFlag",
                table: "Goal",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstCompletionDate",
                table: "Goal",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceTimeFrame",
                table: "Goal",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountNeededEachTimeFrame",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "ExpenseFlag",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "FirstCompletionDate",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "RecurrenceTimeFrame",
                table: "Goal");

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    ExpenseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmountNeededEachTimeFrame = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FundingScheduleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecurrenceTimeFrame = table.Column<long>(type: "bigint", nullable: false),
                    UserAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.ExpenseId);
                });
        }
    }
}
