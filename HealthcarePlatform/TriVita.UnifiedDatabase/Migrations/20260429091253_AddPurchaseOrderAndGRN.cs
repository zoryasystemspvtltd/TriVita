using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrderAndGRN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderItemId",
                schema: "dbo",
                table: "GoodsReceiptItems",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderId",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "SupplierId",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_PurchaseOrderItemId",
                schema: "dbo",
                table: "GoodsReceiptItems",
                column: "PurchaseOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipt_SupplierId",
                schema: "dbo",
                table: "GoodsReceipt",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceipt_Supplier_SupplierId",
                schema: "dbo",
                table: "GoodsReceipt",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                principalSchema: "dbo");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceiptItems_PurchaseOrderItems_PurchaseOrderItemId",
                schema: "dbo",
                table: "GoodsReceiptItems",
                column: "PurchaseOrderItemId",
                principalSchema: "dbo",
                principalTable: "PurchaseOrderItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceipt_Supplier_SupplierId",
                schema: "dbo",
                table: "GoodsReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceiptItems_PurchaseOrderItems_PurchaseOrderItemId",
                schema: "dbo",
                table: "GoodsReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceiptItems_PurchaseOrderItemId",
                schema: "dbo",
                table: "GoodsReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipt_SupplierId",
                schema: "dbo",
                table: "GoodsReceipt");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                schema: "dbo",
                table: "GoodsReceipt");

            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderItemId",
                schema: "dbo",
                table: "GoodsReceiptItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderId",
                schema: "dbo",
                table: "GoodsReceipt",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
