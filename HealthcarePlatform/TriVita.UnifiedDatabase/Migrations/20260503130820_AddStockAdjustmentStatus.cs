using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddStockAdjustmentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StockAdjustment",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("UPDATE StockAdjustment SET Status = 2;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "StockAdjustment");
        }
    }
}
