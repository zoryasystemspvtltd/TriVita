using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[dbo].[Supplier]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[Supplier] (
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
                        CONSTRAINT [PK_dbo_Supplier] PRIMARY KEY ([Id])
                    );

                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_dbo_Supplier_TenantId_Pan' AND object_id = OBJECT_ID(N'[dbo].[Supplier]'))
                        CREATE UNIQUE INDEX [IX_dbo_Supplier_TenantId_Pan] ON [dbo].[Supplier] ([TenantId], [Pan]);

                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_dbo_Supplier_TenantId_SupplierCode' AND object_id = OBJECT_ID(N'[dbo].[Supplier]'))
                        CREATE UNIQUE INDEX [IX_dbo_Supplier_TenantId_SupplierCode] ON [dbo].[Supplier] ([TenantId], [SupplierCode]);

                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_dbo_Supplier_TenantId_SupplierName' AND object_id = OBJECT_ID(N'[dbo].[Supplier]'))
                        CREATE UNIQUE INDEX [IX_dbo_Supplier_TenantId_SupplierName] ON [dbo].[Supplier] ([TenantId], [SupplierName]);
                END

                IF OBJECT_ID(N'[dbo].[Supplier]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[pharmacy].[Supplier]', N'U') IS NOT NULL
                BEGIN
                    SET IDENTITY_INSERT [dbo].[Supplier] ON;

                    INSERT INTO [dbo].[Supplier] (
                        [Id], [TenantId], [FacilityId], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
                        [IsActive], [IsDeleted],
                        [SupplierCode], [SupplierName], [Pan], [Msme], [Tan], [ExportImportCode], [GstNo], [Cin],
                        [ContactPerson], [Phone], [Email], [Address], [Description]
                    )
                    SELECT
                        p.[Id], p.[TenantId], p.[FacilityId], p.[CreatedBy], p.[CreatedOn], p.[ModifiedBy], p.[ModifiedOn],
                        p.[IsActive], p.[IsDeleted],
                        p.[SupplierCode], p.[SupplierName], p.[Pan], p.[Msme], p.[Tan], p.[ExportImportCode], p.[GstNo], p.[Cin],
                        p.[ContactPerson], p.[Phone], p.[Email], p.[Address], p.[Description]
                    FROM [pharmacy].[Supplier] p
                    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Supplier] d WHERE d.[Id] = p.[Id]);

                    SET IDENTITY_INSERT [dbo].[Supplier] OFF;

                    DECLARE @m BIGINT
                    SELECT @m = MAX([Id]) FROM [dbo].[Supplier]
                    IF @m IS NOT NULL DBCC CHECKIDENT ('[dbo].[Supplier]', RESEED, @m)
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[dbo].[Supplier]', N'U') IS NOT NULL
                    DROP TABLE [dbo].[Supplier];
                """);
        }
    }
}
