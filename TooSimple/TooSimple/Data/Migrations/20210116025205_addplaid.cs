using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TooSimple.Data.Migrations
{
    public partial class addplaid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountId = table.Column<string>(nullable: false),
                    AccountTypeId = table.Column<int>(nullable: false),
                    PlaidAccountId = table.Column<int>(nullable: false),
                    Mask = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    CurrentBalance = table.Column<decimal>(nullable: true),
                    AvailableBalance = table.Column<decimal>(nullable: true),
                    CurrencyCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    ExpenseId = table.Column<string>(nullable: false),
                    ExpenseName = table.Column<string>(nullable: true),
                    RecurrenceTimeFrame = table.Column<long>(nullable: false),
                    AmountNeededEachTimeFrame = table.Column<decimal>(nullable: false),
                    CurrentBalance = table.Column<decimal>(nullable: false),
                    FirstCompletionDate = table.Column<DateTime>(nullable: false),
                    FundingScheduleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.ExpenseId);
                });

            migrationBuilder.CreateTable(
                name: "FundingSchedule",
                columns: table => new
                {
                    FundingScheduleId = table.Column<string>(nullable: false),
                    FundingScheduleName = table.Column<string>(nullable: true),
                    Frequency = table.Column<long>(nullable: false),
                    FirstContributionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundingSchedule", x => x.FundingScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "Goal",
                columns: table => new
                {
                    GoalId = table.Column<string>(nullable: false),
                    GoalName = table.Column<string>(nullable: true),
                    GoalAmount = table.Column<decimal>(nullable: false),
                    CurrentBalance = table.Column<decimal>(nullable: false),
                    DesiredCompletionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goal", x => x.GoalId);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    TransactionId = table.Column<string>(nullable: false),
                    PlaidTransactionId = table.Column<string>(nullable: true),
                    PlaidAccountId = table.Column<string>(nullable: true),
                    AccountOwner = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    AuthorizedDate = table.Column<DateTime>(nullable: false),
                    TransactionDate = table.Column<DateTime>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    StoreNumber = table.Column<string>(nullable: true),
                    MerchantName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PaymentChannel = table.Column<string>(nullable: true),
                    ByOrderOf = table.Column<string>(nullable: true),
                    Payee = table.Column<string>(nullable: true),
                    Payer = table.Column<string>(nullable: true),
                    PaymentMethod = table.Column<string>(nullable: true),
                    PaymentProcessor = table.Column<string>(nullable: true),
                    PpdId = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    Pending = table.Column<bool>(nullable: false),
                    PendingTransactionId = table.Column<string>(nullable: true),
                    TransactionCode = table.Column<string>(nullable: true),
                    TransactionType = table.Column<string>(nullable: true),
                    UnofficialCurrencyCode = table.Column<string>(nullable: true),
                    SpendingFrom = table.Column<string>(nullable: true),
                    InternalCategory = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transaction_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionCategory_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_AccountId",
                table: "Transaction",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategory_TransactionId",
                table: "TransactionCategory",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "FundingSchedule");

            migrationBuilder.DropTable(
                name: "Goal");

            migrationBuilder.DropTable(
                name: "TransactionCategory");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
