/* Pharmacy Supplier Master (Unified Database)
   Creates: [pharmacy].[Supplier]
*/

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'pharmacy')
    EXEC(N'CREATE SCHEMA [pharmacy]');
GO

IF OBJECT_ID(N'[pharmacy].[Supplier]', N'U') IS NULL
BEGIN
    CREATE TABLE [pharmacy].[Supplier] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Supplier] PRIMARY KEY,

        [SupplierCode] NVARCHAR(80) NOT NULL,
        [SupplierName] NVARCHAR(250) NOT NULL,
        [Pan] NVARCHAR(10) NOT NULL,
        [Msme] NVARCHAR(50) NULL,
        [Tan] NVARCHAR(20) NULL,
        [ExportImportCode] NVARCHAR(30) NULL,
        [GstNo] NVARCHAR(15) NULL,
        [Cin] NVARCHAR(30) NULL,

        [ContactPerson] NVARCHAR(120) NULL,
        [Phone] NVARCHAR(40) NULL,
        [Email] NVARCHAR(120) NULL,
        [Address] NVARCHAR(300) NULL,
        [Description] NVARCHAR(500) NULL,

        [TenantId] BIGINT NOT NULL,
        [FacilityId] BIGINT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Supplier_IsActive] DEFAULT (1),
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_Supplier_IsDeleted] DEFAULT (0),
        [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_Supplier_CreatedOn] DEFAULT (SYSUTCDATETIME()),
        [CreatedBy] BIGINT NOT NULL CONSTRAINT [DF_Supplier_CreatedBy] DEFAULT (0),
        [ModifiedOn] DATETIME2 NOT NULL CONSTRAINT [DF_Supplier_ModifiedOn] DEFAULT (SYSUTCDATETIME()),
        [ModifiedBy] BIGINT NOT NULL CONSTRAINT [DF_Supplier_ModifiedBy] DEFAULT (0),
        [RowVersion] ROWVERSION NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Supplier_Tenant_SupplierCode' AND object_id = OBJECT_ID(N'[pharmacy].[Supplier]'))
    CREATE UNIQUE INDEX [UX_Supplier_Tenant_SupplierCode] ON [pharmacy].[Supplier]([TenantId], [SupplierCode]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Supplier_Tenant_SupplierName' AND object_id = OBJECT_ID(N'[pharmacy].[Supplier]'))
    CREATE UNIQUE INDEX [UX_Supplier_Tenant_SupplierName] ON [pharmacy].[Supplier]([TenantId], [SupplierName]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Supplier_Tenant_PAN' AND object_id = OBJECT_ID(N'[pharmacy].[Supplier]'))
    CREATE UNIQUE INDEX [UX_Supplier_Tenant_PAN] ON [pharmacy].[Supplier]([TenantId], [Pan]);
GO

