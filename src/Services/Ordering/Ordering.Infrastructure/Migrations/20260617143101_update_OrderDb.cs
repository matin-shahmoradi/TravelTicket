using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_OrderDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Tickets_TicketId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_TicketId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Customers_NationalCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Payment_CVV",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_CardName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_CardNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_Expiration",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_PaymentMethod",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Payment_CVV",
                table: "Orders",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Payment_CardName",
                table: "Orders",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payment_CardNumber",
                table: "Orders",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Payment_Expiration",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Payment_PaymentMethod",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Destination = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Origin = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    SeatNumber = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_TicketId",
                table: "OrderItems",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NationalCode",
                table: "Customers",
                column: "NationalCode");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Tickets_TicketId",
                table: "OrderItems",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
