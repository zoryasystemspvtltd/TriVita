using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddFormMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FormId",
                table: "Medicine",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Form",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_Form", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_FormId",
                table: "Medicine",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Form_TenantId_FormCode",
                table: "Form",
                columns: new[] { "TenantId", "FormCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Form_TenantId_FormName",
                table: "Form",
                columns: new[] { "TenantId", "FormName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medicine_Form_FormId",
                table: "Medicine",
                column: "FormId",
                principalTable: "Form",
                principalColumn: "Id");

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[dbo].[Form]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[dbo].[ReferenceDataValue]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[dbo].[ReferenceDataDefinition]', N'U') IS NOT NULL
                BEGIN
                    SET IDENTITY_INSERT [dbo].[Form] ON;

                    INSERT INTO [dbo].[Form] (
                        [Id], [FormName], [FormCode], [Description],
                        [TenantId], [FacilityId], [IsActive], [IsDeleted],
                        [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]
                    )
                    SELECT
                        v.[Id],
                        LEFT(LTRIM(RTRIM(v.[ValueName])), 300),
                        LEFT(LTRIM(RTRIM(v.[ValueCode])), 80),
                        NULLIF(LEFT(LTRIM(RTRIM(v.[ValueText])), 1000), N''),
                        v.[TenantId], v.[FacilityId], v.[IsActive], v.[IsDeleted],
                        v.[CreatedOn], v.[CreatedBy], v.[ModifiedOn], v.[ModifiedBy]
                    FROM [dbo].[ReferenceDataValue] v
                    INNER JOIN [dbo].[ReferenceDataDefinition] d
                        ON d.[Id] = v.[ReferenceDataDefinitionId]
                       AND d.[TenantId] = v.[TenantId]
                    WHERE d.[DefinitionCode] = N'TRIVITA_PHARMACY_MEDICINE_FORM'
                      AND v.[IsDeleted] = 0
                      AND NOT EXISTS (SELECT 1 FROM [dbo].[Form] f WHERE f.[Id] = v.[Id]);

                    SET IDENTITY_INSERT [dbo].[Form] OFF;
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[dbo].[Medicine]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[dbo].[Medicine]', N'FormReferenceValueId') IS NOT NULL
                BEGIN
                    UPDATE m
                    SET m.[FormId] = m.[FormReferenceValueId]
                    FROM [dbo].[Medicine] m
                    WHERE m.[FormId] IS NULL
                      AND m.[FormReferenceValueId] IS NOT NULL
                      AND EXISTS (SELECT 1 FROM [dbo].[Form] f WHERE f.[Id] = m.[FormReferenceValueId]);
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[dbo].[Medicine]', N'U') IS NOT NULL
                   AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_Medicine_Form' AND parent_object_id = OBJECT_ID(N'[dbo].[Medicine]'))
                BEGIN
                    ALTER TABLE [dbo].[Medicine] DROP CONSTRAINT [FK_Medicine_Form];
                END
                """);

            migrationBuilder.DropColumn(
                name: "FormReferenceValueId",
                table: "Medicine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicine_Form_FormId",
                table: "Medicine");

            migrationBuilder.DropTable(
                name: "Form");

            migrationBuilder.DropIndex(
                name: "IX_Medicine_FormId",
                table: "Medicine");

            migrationBuilder.AddColumn<long>(
                name: "FormReferenceValueId",
                table: "Medicine",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[dbo].[Medicine]', N'U') IS NOT NULL
                BEGIN
                    UPDATE m
                    SET m.[FormReferenceValueId] = m.[FormId]
                    FROM [dbo].[Medicine] m
                    WHERE m.[FormId] IS NOT NULL;
                END
                """);

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "Medicine");
        }
    }
}
