using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations;

public partial class AddCustomerMaster : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Customer",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                MobileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                AlternatePhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                AadhaarNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
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
                table.PrimaryKey("PK_Customer", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Customer_TenantId_AadhaarNumber",
            table: "Customer",
            columns: new[] { "TenantId", "AadhaarNumber" },
            unique: true,
            filter: "[IsDeleted] = 0 AND [AadhaarNumber] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Customer_TenantId_MobileNumber",
            table: "Customer",
            columns: new[] { "TenantId", "MobileNumber" },
            unique: true,
            filter: "[IsDeleted] = 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Customer");
    }
}

