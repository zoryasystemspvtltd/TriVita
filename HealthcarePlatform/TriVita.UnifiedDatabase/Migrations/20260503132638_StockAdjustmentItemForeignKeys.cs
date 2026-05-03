using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class StockAdjustmentItemForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItems_MedicineBatchId",
                table: "StockAdjustmentItems",
                column: "MedicineBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustmentItems_StockAdjustmentId",
                table: "StockAdjustmentItems",
                column: "StockAdjustmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustmentItems_MedicineBatch_MedicineBatchId",
                table: "StockAdjustmentItems",
                column: "MedicineBatchId",
                principalTable: "MedicineBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustmentItems_StockAdjustment_StockAdjustmentId",
                table: "StockAdjustmentItems",
                column: "StockAdjustmentId",
                principalTable: "StockAdjustment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustmentItems_MedicineBatch_MedicineBatchId",
                table: "StockAdjustmentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustmentItems_StockAdjustment_StockAdjustmentId",
                table: "StockAdjustmentItems");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustmentItems_MedicineBatchId",
                table: "StockAdjustmentItems");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustmentItems_StockAdjustmentId",
                table: "StockAdjustmentItems");
        }
    }
}
