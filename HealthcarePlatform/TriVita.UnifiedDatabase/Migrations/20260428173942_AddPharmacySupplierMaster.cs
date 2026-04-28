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
            migrationBuilder.Sql(@"
IF SCHEMA_ID(N'pharmacy') IS NULL
    EXEC(N'CREATE SCHEMA [pharmacy]');

IF OBJECT_ID(N'[pharmacy].[Supplier]', N'U') IS NULL
BEGIN
    CREATE TABLE [pharmacy].[Supplier] (
        [Id] bigint NOT NULL IDENTITY,
        [SupplierCode] nvarchar(80) NOT NULL,
        [SupplierName] nvarchar(250) NOT NULL,
        [Pan] nvarchar(10) NOT NULL,
        [Msme] nvarchar(50) NULL,
        [Tan] nvarchar(20) NULL,
        [ExportImportCode] nvarchar(30) NULL,
        [GstNo] nvarchar(15) NULL,
        [Cin] nvarchar(30) NULL,
        [ContactPerson] nvarchar(120) NULL,
        [Phone] nvarchar(40) NULL,
        [Email] nvarchar(120) NULL,
        [Address] nvarchar(300) NULL,
        [Description] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Supplier] PRIMARY KEY ([Id])
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Supplier_TenantId_Pan' AND object_id = OBJECT_ID(N'[pharmacy].[Supplier]'))
    CREATE UNIQUE INDEX [IX_Supplier_TenantId_Pan] ON [pharmacy].[Supplier] ([TenantId], [Pan]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Supplier_TenantId_SupplierCode' AND object_id = OBJECT_ID(N'[pharmacy].[Supplier]'))
    CREATE UNIQUE INDEX [IX_Supplier_TenantId_SupplierCode] ON [pharmacy].[Supplier] ([TenantId], [SupplierCode]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Supplier_TenantId_SupplierName' AND object_id = OBJECT_ID(N'[pharmacy].[Supplier]'))
    CREATE UNIQUE INDEX [IX_Supplier_TenantId_SupplierName] ON [pharmacy].[Supplier] ([TenantId], [SupplierName]);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[pharmacy].[Supplier]', N'U') IS NOT NULL
    DROP TABLE [pharmacy].[Supplier];
");
        }
    }
}
