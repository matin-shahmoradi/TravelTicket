using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basket.API.Migrations
{
    /// <inheritdoc />
    public partial class updatebasketDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_Username",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "ShoppingCarts");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "ShoppingCarts",
                type: "uuid",
                maxLength: 250,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_CustomerId",
                table: "ShoppingCarts",
                column: "CustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_CustomerId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ShoppingCarts");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ShoppingCarts",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_Username",
                table: "ShoppingCarts",
                column: "Username",
                unique: true);
        }
    }
}
