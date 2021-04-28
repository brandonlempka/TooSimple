using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.DataAccessors.Data.Migrations
{
    public partial class newaccountcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Account",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Account");
        }
    }
}
