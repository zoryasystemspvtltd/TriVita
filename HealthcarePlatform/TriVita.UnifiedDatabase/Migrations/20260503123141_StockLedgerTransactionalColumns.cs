using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class StockLedgerTransactionalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionOn",
                table: "StockLedger",
                newName: "TransactionDate");

            migrationBuilder.AddColumn<long>(
                name: "MedicineId",
                table: "StockLedger",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReferenceId",
                table: "StockLedger",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReferenceLineId",
                table: "StockLedger",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "TransactionType",
                table: "StockLedger",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)1);

            migrationBuilder.Sql(
                """
                UPDATE sl
                SET MedicineId = mb.MedicineId
                FROM StockLedger AS sl
                INNER JOIN MedicineBatch AS mb ON mb.Id = sl.MedicineBatchId
                WHERE sl.MedicineId IS NULL;
                """);

            migrationBuilder.Sql(
                """
                UPDATE sl
                SET MedicineId = m.Id
                FROM StockLedger AS sl
                CROSS APPLY (
                    SELECT TOP (1) Id FROM Medicine WHERE IsDeleted = 0 ORDER BY Id
                ) AS m
                WHERE sl.MedicineId IS NULL
                  AND EXISTS (SELECT 1 FROM Medicine WHERE IsDeleted = 0);
                """);

            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM StockLedger WHERE MedicineId IS NULL)
                BEGIN
                    THROW 50001, 'StockLedger has rows that could not resolve MedicineId from MedicineBatch or Medicine. Repair or remove those rows, then re-run the migration.', 1;
                END
                """);

            migrationBuilder.AlterColumn<long>(
                name: "MedicineId",
                table: "StockLedger",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_MedicineBatchId",
                table: "StockLedger",
                column: "MedicineBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_MedicineId",
                table: "StockLedger",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_TenantId_FacilityId_TransactionDate",
                table: "StockLedger",
                columns: new[] { "TenantId", "FacilityId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_TenantId_MedicineId_TransactionDate",
                table: "StockLedger",
                columns: new[] { "TenantId", "MedicineId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StockLedger_TenantId_ReferenceId_TransactionType",
                table: "StockLedger",
                columns: new[] { "TenantId", "ReferenceId", "TransactionType" });

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedger_MedicineBatch_MedicineBatchId",
                table: "StockLedger",
                column: "MedicineBatchId",
                principalTable: "MedicineBatch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedger_Medicine_MedicineId",
                table: "StockLedger",
                column: "MedicineId",
                principalTable: "Medicine",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockLedger_MedicineBatch_MedicineBatchId",
                table: "StockLedger");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedger_Medicine_MedicineId",
                table: "StockLedger");

            migrationBuilder.DropIndex(
                name: "IX_StockLedger_MedicineBatchId",
                table: "StockLedger");

            migrationBuilder.DropIndex(
                name: "IX_StockLedger_MedicineId",
                table: "StockLedger");

            migrationBuilder.DropIndex(
                name: "IX_StockLedger_TenantId_FacilityId_TransactionDate",
                table: "StockLedger");

            migrationBuilder.DropIndex(
                name: "IX_StockLedger_TenantId_MedicineId_TransactionDate",
                table: "StockLedger");

            migrationBuilder.DropIndex(
                name: "IX_StockLedger_TenantId_ReferenceId_TransactionType",
                table: "StockLedger");

            migrationBuilder.DropColumn(
                name: "MedicineId",
                table: "StockLedger");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "StockLedger");

            migrationBuilder.DropColumn(
                name: "ReferenceLineId",
                table: "StockLedger");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "StockLedger");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "StockLedger",
                newName: "TransactionOn");
        }
    }
}
