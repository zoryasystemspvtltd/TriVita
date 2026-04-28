using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacySupplierMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Supplier",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Pan = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Msme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ExportImportCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    GstNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Cin = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_TenantId_Pan",
                schema: "pharmacy",
                table: "Supplier",
                columns: new[] { "TenantId", "Pan" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_TenantId_SupplierCode",
                schema: "pharmacy",
                table: "Supplier",
                columns: new[] { "TenantId", "SupplierCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_TenantId_SupplierName",
                schema: "pharmacy",
                table: "Supplier",
                columns: new[] { "TenantId", "SupplierName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Supplier",
                schema: "pharmacy");
        }
    }
}
