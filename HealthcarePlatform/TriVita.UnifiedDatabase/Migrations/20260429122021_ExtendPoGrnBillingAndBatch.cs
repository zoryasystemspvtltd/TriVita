using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class ExtendPoGrnBillingAndBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LineTotal",
                schema: "dbo",
                table: "PurchaseOrderItems",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                schema: "dbo",
                table: "PurchaseOrder",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GstAmount",
                schema: "dbo",
                table: "PurchaseOrder",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GstPercent",
                schema: "dbo",
                table: "PurchaseOrder",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherTaxAmount",
                schema: "dbo",
                table: "PurchaseOrder",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                schema: "dbo",
                table: "PurchaseOrder",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                schema: "dbo",
                table: "PurchaseOrder",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantity",
                schema: "dbo",
                table: "MedicineBatch",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "CreatedFromGoodsReceiptId",
                schema: "dbo",
                table: "MedicineBatch",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LineTotal",
                schema: "dbo",
                table: "GoodsReceiptItems",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GstAmount",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GstPercent",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherTaxAmount",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_MedicineBatch_CreatedFromGoodsReceiptId",
                schema: "dbo",
                table: "MedicineBatch",
                column: "CreatedFromGoodsReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineBatch_TenantId_MedicineId_BatchNo",
                schema: "dbo",
                table: "MedicineBatch",
                columns: new[] { "TenantId", "MedicineId", "BatchNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineBatch_GoodsReceipt_CreatedFromGoodsReceiptId",
                schema: "dbo",
                table: "MedicineBatch",
                column: "CreatedFromGoodsReceiptId",
                principalSchema: "dbo",
                principalTable: "GoodsReceipt",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineBatch_GoodsReceipt_CreatedFromGoodsReceiptId",
                schema: "dbo",
                table: "MedicineBatch");

            migrationBuilder.DropIndex(
                name: "IX_MedicineBatch_CreatedFromGoodsReceiptId",
                schema: "dbo",
                table: "MedicineBatch");

            migrationBuilder.DropIndex(
                name: "IX_MedicineBatch_TenantId_MedicineId_BatchNo",
                schema: "dbo",
                table: "MedicineBatch");

            migrationBuilder.DropColumn(
                name: "LineTotal",
                schema: "dbo",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                schema: "dbo",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "GstAmount",
                schema: "dbo",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "GstPercent",
                schema: "dbo",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "OtherTaxAmount",
                schema: "dbo",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                schema: "dbo",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                schema: "dbo",
                table: "MedicineBatch");

            migrationBuilder.DropColumn(
                name: "CreatedFromGoodsReceiptId",
                schema: "dbo",
                table: "MedicineBatch");

            migrationBuilder.DropColumn(
                name: "LineTotal",
                schema: "dbo",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                schema: "dbo",
                table: "GoodsReceipt");

            migrationBuilder.DropColumn(
                name: "GstAmount",
                schema: "dbo",
                table: "GoodsReceipt");

            migrationBuilder.DropColumn(
                name: "GstPercent",
                schema: "dbo",
                table: "GoodsReceipt");

            migrationBuilder.DropColumn(
                name: "OtherTaxAmount",
                schema: "dbo",
                table: "GoodsReceipt");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                schema: "dbo",
                table: "GoodsReceipt");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                schema: "dbo",
                table: "GoodsReceipt");
        }
    }
}
