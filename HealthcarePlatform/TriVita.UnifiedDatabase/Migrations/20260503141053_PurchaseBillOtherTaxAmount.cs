using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class PurchaseBillOtherTaxAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OtherTaxAmount",
                table: "PurchaseBill",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherTaxAmount",
                table: "PurchaseBill");
        }
    }
}
