using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.DataAccessors.Data.Migrations
{
    public partial class remveplaidtxid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaidTransactionId",
                table: "Transaction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlaidTransactionId",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
