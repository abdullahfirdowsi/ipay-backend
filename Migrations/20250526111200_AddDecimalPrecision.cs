using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillPaymentBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AdminVerificationDate",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdminVerified",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RazorpayOrderId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazorpayPaymentId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazorpaySignature",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminComments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminVerifications_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillPaymentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPaymentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillPaymentDetails_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId");
                });

            migrationBuilder.CreateTable(
                name: "RazorpayOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorpayOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RazorpayOrders_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RazorpayPaymentId",
                table: "Transactions",
                column: "RazorpayPaymentId",
                unique: true,
                filter: "[RazorpayPaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AdminVerifications_TransactionId",
                table: "AdminVerifications",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillPaymentDetails_TransactionId",
                table: "BillPaymentDetails",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RazorpayOrders_OrderId",
                table: "RazorpayOrders",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RazorpayOrders_TransactionId",
                table: "RazorpayOrders",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminVerifications");

            migrationBuilder.DropTable(
                name: "BillPaymentDetails");

            migrationBuilder.DropTable(
                name: "RazorpayOrders");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_RazorpayPaymentId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AdminVerificationDate",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsAdminVerified",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RazorpayOrderId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RazorpayPaymentId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RazorpaySignature",
                table: "Transactions");
        }
    }
}
