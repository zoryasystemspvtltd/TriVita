using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class MigratePharmacySupplierFromReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // One-time copy from pharmacy.ReferenceDataValue (TRIVITA_PHARMACY_SUPPLIER) into pharmacy.Supplier,
            // preserving Ids. Soft-delete migrated reference rows so new supplier data uses Supplier only.
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[pharmacy].[Supplier]', N'U') IS NULL
                   OR OBJECT_ID(N'[pharmacy].[ReferenceDataValue]', N'U') IS NULL
                   OR OBJECT_ID(N'[pharmacy].[ReferenceDataDefinition]', N'U') IS NULL
                    RETURN;

                SET IDENTITY_INSERT [pharmacy].[Supplier] ON;

                ;WITH Src AS (
                    SELECT v.*
                    FROM [pharmacy].[ReferenceDataValue] v
                    INNER JOIN [pharmacy].[ReferenceDataDefinition] d
                        ON v.[TenantId] = d.[TenantId] AND v.[ReferenceDataDefinitionId] = d.[Id]
                    WHERE d.[DefinitionCode] = N'TRIVITA_PHARMACY_SUPPLIER'
                      AND v.[IsDeleted] = 0
                ),
                Split AS (
                    SELECT s.[Id],
                           LTRIM(RTRIM(CAST(ss.[value] AS NVARCHAR(MAX)))) AS [line]
                    FROM Src s
                    CROSS APPLY STRING_SPLIT(REPLACE(ISNULL(s.[ValueText], N''), NCHAR(13), N''), NCHAR(10)) ss
                ),
                Agg AS (
                    SELECT
                        s.[Id],
                        s.[TenantId],
                        s.[FacilityId],
                        s.[CreatedBy],
                        s.[CreatedOn],
                        s.[ModifiedBy],
                        s.[ModifiedOn],
                        s.[IsActive],
                        s.[IsDeleted],
                        s.[ValueCode],
                        s.[ValueName],
                        MAX(CASE WHEN sp.[line] LIKE N'PAN:%' OR sp.[line] LIKE N'PAN :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS PanX,
                        MAX(CASE WHEN sp.[line] LIKE N'MSME:%' OR sp.[line] LIKE N'MSME :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS MsmeX,
                        MAX(CASE WHEN sp.[line] LIKE N'TAN:%' OR sp.[line] LIKE N'TAN :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS TanX,
                        MAX(CASE WHEN sp.[line] LIKE N'IEC:%' OR sp.[line] LIKE N'IEC :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS IecX,
                        MAX(CASE WHEN sp.[line] LIKE N'GST:%' OR sp.[line] LIKE N'GST :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS GstX,
                        MAX(CASE WHEN sp.[line] LIKE N'CIN:%' OR sp.[line] LIKE N'CIN :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS CinX,
                        MAX(CASE WHEN sp.[line] LIKE N'Contact:%' OR sp.[line] LIKE N'Contact :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS ContactX,
                        MAX(CASE WHEN sp.[line] LIKE N'Phone:%' OR sp.[line] LIKE N'Phone :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS PhoneX,
                        MAX(CASE WHEN sp.[line] LIKE N'Email:%' OR sp.[line] LIKE N'Email :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS EmailX,
                        MAX(CASE WHEN sp.[line] LIKE N'Address:%' OR sp.[line] LIKE N'Address :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS AddressX,
                        MAX(CASE WHEN sp.[line] LIKE N'Description:%' OR sp.[line] LIKE N'Description :%'
                            THEN LTRIM(RTRIM(SUBSTRING(sp.[line], CHARINDEX(N':', sp.[line]) + 1, 500))) END) AS DescX
                    FROM Src s
                    LEFT JOIN Split sp ON sp.[Id] = s.[Id]
                    GROUP BY s.[Id], s.[TenantId], s.[FacilityId], s.[CreatedBy], s.[CreatedOn], s.[ModifiedBy], s.[ModifiedOn],
                             s.[IsActive], s.[IsDeleted], s.[ValueCode], s.[ValueName]
                )
                INSERT INTO [pharmacy].[Supplier] (
                    [Id], [TenantId], [FacilityId], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
                    [IsActive], [IsDeleted],
                    [SupplierCode], [SupplierName], [Pan], [Msme], [Tan], [ExportImportCode], [GstNo], [Cin],
                    [ContactPerson], [Phone], [Email], [Address], [Description]
                )
                SELECT
                    a.[Id],
                    a.[TenantId],
                    a.[FacilityId],
                    a.[CreatedBy],
                    a.[CreatedOn],
                    a.[ModifiedBy],
                    a.[ModifiedOn],
                    a.[IsActive],
                    a.[IsDeleted],
                    a.[ValueCode],
                    a.[ValueName],
                    CASE
                        WHEN a.PanX IS NOT NULL AND LEN(LTRIM(RTRIM(a.PanX))) = 10 THEN LEFT(LTRIM(RTRIM(a.PanX)), 10)
                        ELSE N'AAAAA0000A'
                    END,
                    NULLIF(LEFT(LTRIM(RTRIM(a.MsmeX)), 50), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.TanX)), 20), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.IecX)), 30), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.GstX)), 15), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.CinX)), 30), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.ContactX)), 120), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.PhoneX)), 40), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.EmailX)), 120), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.AddressX)), 300), N''),
                    NULLIF(LEFT(LTRIM(RTRIM(a.DescX)), 500), N'')
                FROM Agg a
                WHERE NOT EXISTS (SELECT 1 FROM [pharmacy].[Supplier] s WHERE s.[Id] = a.[Id]);

                SET IDENTITY_INSERT [pharmacy].[Supplier] OFF;

                UPDATE v
                SET v.[IsDeleted] = 1,
                    v.[IsActive] = 0,
                    v.[ModifiedOn] = SYSUTCDATETIME()
                FROM [pharmacy].[ReferenceDataValue] v
                INNER JOIN [pharmacy].[ReferenceDataDefinition] d
                    ON v.[TenantId] = d.[TenantId] AND v.[ReferenceDataDefinitionId] = d.[Id]
                WHERE d.[DefinitionCode] = N'TRIVITA_PHARMACY_SUPPLIER'
                  AND EXISTS (SELECT 1 FROM [pharmacy].[Supplier] s WHERE s.[Id] = v.[Id]);
                """);

            // Separate batch: avoid ';' inside dynamic EXEC strings (EF splits Sql on ';').
            migrationBuilder.Sql(
                """
                DECLARE @m BIGINT
                SELECT @m = MAX([Id]) FROM [pharmacy].[Supplier]
                IF @m IS NOT NULL DBCC CHECKIDENT ('[pharmacy].[Supplier]', RESEED, @m)
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data migration is not reversed automatically.
        }
    }
}
