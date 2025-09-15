using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanCode.ORM.Migrations
{
    /// <inheritdoc />
    public partial class AddCashFlowEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalCredits = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalDebits = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ClosingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreditTransactionCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DebitTransactionCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalTransactionCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyBalances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyBalances_Date_NonUnique",
                table: "DailyBalances",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Category",
                table: "Transactions",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Date_Type",
                table: "Transactions",
                columns: new[] { "TransactionDate", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Type",
                table: "Transactions",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyBalances");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
