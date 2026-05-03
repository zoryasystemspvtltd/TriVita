using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseBill",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: true),
                    GoodsReceiptId = table.Column<long>(type: "bigint", nullable: false),
                    SourceMode = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    GstPercent = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    GstAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseBill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseBill_GoodsReceipt_GoodsReceiptId",
                        column: x => x.GoodsReceiptId,
                        principalTable: "GoodsReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseBill_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseBill_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseBillItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseBillId = table.Column<long>(type: "bigint", nullable: false),
                    GoodsReceiptId = table.Column<long>(type: "bigint", nullable: false),
                    GoodsReceiptItemId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseBillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseBillItems_GoodsReceiptItems_GoodsReceiptItemId",
                        column: x => x.GoodsReceiptItemId,
                        principalTable: "GoodsReceiptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseBillItems_GoodsReceipt_GoodsReceiptId",
                        column: x => x.GoodsReceiptId,
                        principalTable: "GoodsReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseBillItems_PurchaseBill_PurchaseBillId",
                        column: x => x.PurchaseBillId,
                        principalTable: "PurchaseBill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_GoodsReceiptId",
                table: "PurchaseBill",
                column: "GoodsReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_PurchaseOrderId",
                table: "PurchaseBill",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_SupplierId",
                table: "PurchaseBill",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_TenantId_SupplierId_InvoiceNo",
                table: "PurchaseBill",
                columns: new[] { "TenantId", "SupplierId", "InvoiceNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBillItems_GoodsReceiptId",
                table: "PurchaseBillItems",
                column: "GoodsReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBillItems_GoodsReceiptItemId",
                table: "PurchaseBillItems",
                column: "GoodsReceiptItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBillItems_PurchaseBillId",
                table: "PurchaseBillItems",
                column: "PurchaseBillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseBillItems");

            migrationBuilder.DropTable(
                name: "PurchaseBill");
        }
    }
}
