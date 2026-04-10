using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriVita.UnifiedDatabase.Migrations
{
    /// <inheritdoc />
    public partial class InitialUnifiedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.EnsureSchema(
                name: "pharmacy");

            migrationBuilder.EnsureSchema(
                name: "communication");

            migrationBuilder.EnsureSchema(
                name: "lms");

            migrationBuilder.EnsureSchema(
                name: "hms");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.EnsureSchema(
                name: "lis");

            migrationBuilder.CreateTable(
                name: "Address",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    AddressType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Line1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Line2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateProvince = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.UniqueConstraint("AK_Address_TenantId_Id", x => new { x.TenantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "BatchStock",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineBatchId = table.Column<long>(type: "bigint", nullable: false),
                    CurrentQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ReservedQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AvailableQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ReorderLevelQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_BatchStock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COM_Notification",
                schema: "communication",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true),
                    ContextJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriorityReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_COM_Notification", x => x.Id);
                    table.UniqueConstraint("AK_COM_Notification_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "COM_NotificationTemplate",
                schema: "communication",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ChannelTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_COM_NotificationTemplate", x => x.Id);
                    table.UniqueConstraint("AK_COM_NotificationTemplate_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Composition",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompositionName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CompositionCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Composition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactDetails",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    ContactType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactValue = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactDetails", x => x.Id);
                    table.UniqueConstraint("AK_ContactDetails_TenantId_Id", x => new { x.TenantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EquipmentName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EquipmentTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    EquipmentNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                    table.UniqueConstraint("AK_Equipment_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCalibration",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false),
                    CalibratedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CalibratorDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    ResultNumeric = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ResultText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsWithinTolerance = table.Column<bool>(type: "bit", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_EquipmentCalibration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMaintenance",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false),
                    MaintenanceTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    MaintenanceStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    ScheduledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    MaintenanceNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextDueOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EquipmentMaintenance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpiryTracking",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineBatchId = table.Column<long>(type: "bigint", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: false),
                    DaysToExpiry = table.Column<int>(type: "int", nullable: true),
                    ExpiryAlertStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    LastReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_ExpiryTracking", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceipt",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: false),
                    ReceivedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_GoodsReceipt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptItems",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseOrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<long>(type: "bigint", nullable: false),
                    MedicineBatchId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityReceived = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    PurchaseRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    MRP = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                    table.PrimaryKey("PK_GoodsReceiptItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_AppointmentQueue",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    QueueToken = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PositionInQueue = table.Column<int>(type: "int", nullable: false),
                    QueueStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    EnqueuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckedInOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedServiceOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_HMS_AppointmentQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_AppointmentStatusHistory",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    StatusValueId = table.Column<long>(type: "bigint", nullable: false),
                    StatusOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_HMS_AppointmentStatusHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_BillingHeader",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    VisitId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TaxTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DiscountTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_BillingHeader", x => x.Id);
                    table.UniqueConstraint("AK_HMS_BillingHeader_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "HMS_BillingItems",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillingHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    ServiceTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    LineTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: true),
                    PrescriptionId = table.Column<long>(type: "bigint", nullable: true),
                    PharmacySalesId = table.Column<long>(type: "bigint", nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
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
                    table.PrimaryKey("PK_HMS_BillingItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_ClinicalNotes",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<long>(type: "bigint", nullable: false),
                    NoteTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    EncounterSection = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NoteText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StructuredPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_HMS_ClinicalNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_Diagnosis",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<long>(type: "bigint", nullable: false),
                    DiagnosisTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    ICDSystem = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ICDCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ICDVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ICDDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DiagnosisOn = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_HMS_Diagnosis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_InsuranceProvider",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TpaCategoryReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_HMS_InsuranceProvider", x => x.Id);
                    table.UniqueConstraint("AK_HMS_InsuranceProvider_TenantId_Id", x => new { x.TenantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "HMS_OperationTheatre",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheatreCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TheatreName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_OperationTheatre", x => x.Id);
                    table.UniqueConstraint("AK_HMS_OperationTheatre_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "HMS_PackageDefinition",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PackageName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BundlePrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_PackageDefinition", x => x.Id);
                    table.UniqueConstraint("AK_HMS_PackageDefinition_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "HMS_PatientMaster",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UPID = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SharedPatientId = table.Column<long>(type: "bigint", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenderReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    PrimaryEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_HMS_PatientMaster", x => x.Id);
                    table.UniqueConstraint("AK_HMS_PatientMaster_TenantId_Id", x => new { x.TenantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "HMS_PaymentModes",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModeCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ModeName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_HMS_PaymentModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_PaymentTransactions",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillingHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentModeId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TransactionOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_HMS_PaymentTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_Prescription",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    VisitId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    DoctorId = table.Column<long>(type: "bigint", nullable: false),
                    PrescribedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrescriptionStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Indication = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_HMS_Prescription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_PrescriptionItems",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DosageText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FrequencyText = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DurationDays = table.Column<int>(type: "int", nullable: true),
                    RouteReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    IsPRN = table.Column<bool>(type: "bit", nullable: false),
                    ItemNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_HMS_PrescriptionItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_PrescriptionNotes",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<long>(type: "bigint", nullable: false),
                    NoteTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    NoteText = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_HMS_PrescriptionNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_PricingRule",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TariffTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_PricingRule", x => x.Id);
                    table.UniqueConstraint("AK_HMS_PricingRule_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "HMS_Procedure",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<long>(type: "bigint", nullable: false),
                    ProcedureCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProcedureSystem = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ProcedureDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_HMS_Procedure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_VisitType",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitTypeCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    VisitTypeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_HMS_VisitType", x => x.Id);
                    table.UniqueConstraint("AK_HMS_VisitType_TenantId_Id", x => new { x.TenantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "HMS_Vitals",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<long>(type: "bigint", nullable: false),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VitalReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ValueNumeric = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ValueNumeric2 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ValueText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    RecordedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_HMS_Vitals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HMS_Ward",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WardCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    WardName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WardCategoryReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_Ward", x => x.Id);
                    table.UniqueConstraint("AK_HMS_Ward_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "IAM_PasswordResetToken",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestChannelReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_IAM_PasswordResetToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IAM_Permission",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PermissionName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ModuleCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
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
                    table.PrimaryKey("PK_IAM_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IAM_Role",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_IAM_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IAM_RolePermission",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_IAM_RolePermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IAM_UserAccount",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PatientId = table.Column<long>(type: "bigint", nullable: true),
                    DoctorId = table.Column<long>(type: "bigint", nullable: true),
                    UserStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    LastLoginOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationSourceReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_IAM_UserAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IAM_UserFacilityScope",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GrantFacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_IAM_UserFacilityScope", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IAM_UserMfaFactor",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    MfaTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    SecretPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    LastUsedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_IAM_UserMfaFactor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IAM_UserRoleAssignment",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessUnitId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_IAM_UserRoleAssignment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IAM_UserSessionActivity",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionTokenHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ClientIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_IAM_UserSessionActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Identity_Permission",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PermissionName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ModuleCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_Permission", x => x.Id);
                    table.UniqueConstraint("AK_Identity_Permission_TenantId_Id", x => new { x.TenantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Identity_Role",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    RoleCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_Role", x => x.Id);
                    table.UniqueConstraint("AK_Identity_Role_TenantId_Id", x => new { x.TenantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Identity_Users",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabInventory",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryItemCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    InventoryItemName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    CurrentQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
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
                    table.PrimaryKey("PK_LabInventory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabInventoryTransactions",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabInventoryId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    QuantityDelta = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TransactionOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LabInventoryTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_AnalyzerResultHeader",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarcodeValue = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    LmsTestBookingItemId = table.Column<long>(type: "bigint", nullable: false),
                    LmsCatalogTestId = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentAssayCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ReceivedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TechnicallyVerified = table.Column<bool>(type: "bit", nullable: false),
                    TechnicallyVerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadyForDispatch = table.Column<bool>(type: "bit", nullable: false),
                    ResultStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LIS_AnalyzerResultHeader", x => x.Id);
                    table.UniqueConstraint("AK_LIS_AnalyzerResultHeader_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LIS_AnalyzerResultMap",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalTestCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ExternalParameterCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: false),
                    TestParameterId = table.Column<long>(type: "bigint", nullable: true),
                    ProtocolReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    UnitOverrideId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LIS_AnalyzerResultMap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_LabOrder",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    VisitId = table.Column<long>(type: "bigint", nullable: true),
                    OrderingDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    OrderedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    PriorityReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    ClinicalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedCollectionOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_LIS_LabOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_LabOrderItems",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: false),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: false),
                    SampleTypeId = table.Column<long>(type: "bigint", nullable: true),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderItemStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    SpecimenVolume = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SpecimenVolumeUnitId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LIS_LabOrderItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_LabResults",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    TestParameterId = table.Column<long>(type: "bigint", nullable: false),
                    ResultNumeric = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ResultText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultUnitId = table.Column<long>(type: "bigint", nullable: true),
                    IsAbnormal = table.Column<bool>(type: "bit", nullable: false),
                    AbnormalFlagReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MeasuredOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResultStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    EnteredByDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LIS_LabResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_OrderStatusHistory",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: false),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    StatusOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LIS_OrderStatusHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_ReportDeliveryOtp",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    OtpHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryChannelReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LIS_ReportDeliveryOtp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_ReportDetails",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: true),
                    TestParameterId = table.Column<long>(type: "bigint", nullable: true),
                    ResultDisplayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceRangeDisplayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_LIS_ReportDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_ReportHeader",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: false),
                    ReportNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ReportTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    ReportStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    PreparedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreparedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    IssuedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LIS_ReportHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_ReportLockState",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    LockReasonReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LIS_ReportLockState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_ResultApproval",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabResultsId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovalStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedByDoctorId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_LIS_ResultApproval", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_ResultHistory",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabResultsId = table.Column<long>(type: "bigint", nullable: false),
                    SnapshotResultNumeric = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SnapshotResultText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotIsAbnormal = table.Column<bool>(type: "bit", nullable: false),
                    SnapshotAbnormalFlagReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    SnapshotResultStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ChangedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_LIS_ResultHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_SampleBarcode",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCollectionId = table.Column<long>(type: "bigint", nullable: false),
                    BarcodeValue = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    QrPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentifierTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    PrintedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_LIS_SampleBarcode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_SampleCollection",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    SampleTypeId = table.Column<long>(type: "bigint", nullable: false),
                    CollectedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CollectedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    CollectionDepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    CollectedQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CollectedQuantityUnitId = table.Column<long>(type: "bigint", nullable: true),
                    CollectionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_LIS_SampleCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_SampleLifecycleEvent",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCollectionId = table.Column<long>(type: "bigint", nullable: false),
                    LabOrderItemId = table.Column<long>(type: "bigint", nullable: true),
                    EventTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    EventOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedDueOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TatBreached = table.Column<bool>(type: "bit", nullable: false),
                    LocationDepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    EventNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_LIS_SampleLifecycleEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_SampleTracking",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleCollectionId = table.Column<long>(type: "bigint", nullable: false),
                    TrackingNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TrackingEventTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    TrackingStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    LocationDepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    TrackedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScannedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    TrackingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_LIS_SampleTracking", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_SampleType",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleTypeCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SampleTypeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_LIS_SampleType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_TestCategory",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ParentCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_LIS_TestCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_TestMaster",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    SampleTypeId = table.Column<long>(type: "bigint", nullable: true),
                    TestCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TestDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultUnitId = table.Column<long>(type: "bigint", nullable: true),
                    IsQuantitative = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_LIS_TestMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_TestParameterProfile",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestParameterId = table.Column<long>(type: "bigint", nullable: false),
                    MethodReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    CollectionMethodReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    ContainerTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    AnalyzerChannelCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    LoincCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LIS_TestParameterProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_TestParameters",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsNumeric = table.Column<bool>(type: "bit", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    ParameterNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_LIS_TestParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LIS_TestReferenceRanges",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestParameterId = table.Column<long>(type: "bigint", nullable: false),
                    SexReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    AgeFromYears = table.Column<int>(type: "int", nullable: true),
                    AgeToYears = table.Column<int>(type: "int", nullable: true),
                    ReferenceRangeTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    RangeUnitId = table.Column<long>(type: "bigint", nullable: true),
                    RangeNotes = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    EffectiveFromDate = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveToDate = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_LIS_TestReferenceRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_AnalyticsDailyFacilityRollup",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LabOrderCount = table.Column<int>(type: "int", nullable: false),
                    ReportIssuedCount = table.Column<int>(type: "int", nullable: false),
                    GrossRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferralFeeAccrued = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvgTatMinutes = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_LMS_AnalyticsDailyFacilityRollup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_B2BCreditLedger",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    B2BPartnerCreditProfileId = table.Column<long>(type: "bigint", nullable: false),
                    MovementTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    AmountDelta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PostedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LabInvoiceHeaderId = table.Column<long>(type: "bigint", nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LMS_B2BCreditLedger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_B2BPartner",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PartnerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PartnerCategoryReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryAddressId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryContactId = table.Column<long>(type: "bigint", nullable: true),
                    ContractReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_LMS_B2BPartner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_B2BPartnerBillingStatement",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatementNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    B2BPartnerId = table.Column<long>(type: "bigint", nullable: false),
                    PeriodStartOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEndOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatementStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_LMS_B2BPartnerBillingStatement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_B2BPartnerBillingStatementLine",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerBillingStatementId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    LabInvoiceHeaderId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_LMS_B2BPartnerBillingStatementLine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_B2BPartnerCreditProfile",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    B2BPartnerId = table.Column<long>(type: "bigint", nullable: false),
                    CreditLimitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    PaymentTermsDays = table.Column<int>(type: "int", nullable: true),
                    GracePeriodDays = table.Column<int>(type: "int", nullable: true),
                    UtilizedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_LMS_B2BPartnerCreditProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_B2BPartnerTestRate",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    B2BPartnerId = table.Column<long>(type: "bigint", nullable: false),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: false),
                    RateAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractDocumentRef = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_LMS_B2BPartnerTestRate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_CatalogParameter",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsNumeric = table.Column<bool>(type: "bit", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    ParameterNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LMS_CatalogParameter", x => x.Id);
                    table.UniqueConstraint("AK_LMS_CatalogParameter_TenantId_Id", x => new { x.TenantId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LMS_CatalogTest",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TestDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DisciplineReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    SampleTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    IsRadiology = table.Column<bool>(type: "bit", nullable: false),
                    DefaultUnitId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_CatalogTest", x => x.Id);
                    table.UniqueConstraint("AK_LMS_CatalogTest_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LMS_CollectionRequest",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    CollectionAddressJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedWindowStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedWindowEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ColdChainRequired = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_CollectionRequest", x => x.Id);
                    table.UniqueConstraint("AK_LMS_CollectionRequest_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LMS_EquipmentType",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
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
                    table.PrimaryKey("PK_LMS_EquipmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_FinanceLedgerEntry",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountCategoryReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    SourceTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebitCreditReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    PatientId = table.Column<long>(type: "bigint", nullable: true),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LMS_FinanceLedgerEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_LabInvoiceHeader",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    VisitId = table.Column<long>(type: "bigint", nullable: true),
                    InvoiceStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceDue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
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
                    table.PrimaryKey("PK_LMS_LabInvoiceHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_LabInvoiceLine",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabInvoiceHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    LineTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    LabOrderItemId = table.Column<long>(type: "bigint", nullable: true),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: true),
                    TestPackageId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LineSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
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
                    table.PrimaryKey("PK_LMS_LabInvoiceLine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_LabOrderContext",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: false),
                    B2BPartnerId = table.Column<long>(type: "bigint", nullable: true),
                    ReferralDoctorProfileId = table.Column<long>(type: "bigint", nullable: true),
                    SampleSourceReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    BookingChannelReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    ExpectedReportOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_LMS_LabOrderContext", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_LabPaymentTransaction",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabInvoiceHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentModeId = table.Column<long>(type: "bigint", nullable: true),
                    GatewayProviderReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    ExternalTransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LMS_LabPaymentTransaction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_LabTestBooking",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    VisitId = table.Column<long>(type: "bigint", nullable: true),
                    SourceReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    BookingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_LabTestBooking", x => x.Id);
                    table.UniqueConstraint("AK_LMS_LabTestBooking_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LMS_PatientWalletAccount",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_LMS_PatientWalletAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_PatientWalletTransaction",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientWalletAccountId = table.Column<long>(type: "bigint", nullable: false),
                    AmountDelta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalletTxnTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LabInvoiceHeaderId = table.Column<long>(type: "bigint", nullable: true),
                    LabPaymentTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LMS_PatientWalletTransaction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReagentBatch",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReagentMasterId = table.Column<long>(type: "bigint", nullable: false),
                    LotNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LabInventoryId = table.Column<long>(type: "bigint", nullable: true),
                    OpeningQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
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
                    table.PrimaryKey("PK_LMS_ReagentBatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReagentConsumptionLog",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReagentBatchId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityConsumed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConsumedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LabOrderItemId = table.Column<long>(type: "bigint", nullable: true),
                    WorkQueueId = table.Column<long>(type: "bigint", nullable: true),
                    ConsumptionReasonReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LMS_ReagentConsumptionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReagentMaster",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReagentCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ReagentName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DefaultUnitId = table.Column<long>(type: "bigint", nullable: true),
                    StorageNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LMS_ReagentMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReferralDoctorProfile",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferralCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LinkedDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    HospitalAffiliation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PrimaryContactId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryAddressId = table.Column<long>(type: "bigint", nullable: true),
                    ReferralTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LMS_ReferralDoctorProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReferralFeeLedger",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferralDoctorProfileId = table.Column<long>(type: "bigint", nullable: false),
                    LabInvoiceHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    LabInvoiceLineId = table.Column<long>(type: "bigint", nullable: true),
                    LabOrderItemId = table.Column<long>(type: "bigint", nullable: true),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LedgerStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    AccruedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_LMS_ReferralFeeLedger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReferralFeeRule",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferralDoctorProfileId = table.Column<long>(type: "bigint", nullable: false),
                    FeeModeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    FeeValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApplyScopeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_LMS_ReferralFeeRule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReferralSettlement",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettlementNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ReferralDoctorProfileId = table.Column<long>(type: "bigint", nullable: false),
                    PeriodStartOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEndOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalSettledAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SettlementStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    SettledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentReferenceNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
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
                    table.PrimaryKey("PK_LMS_ReferralSettlement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReferralSettlementLine",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferralSettlementId = table.Column<long>(type: "bigint", nullable: false),
                    ReferralFeeLedgerId = table.Column<long>(type: "bigint", nullable: false),
                    AppliedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_LMS_ReferralSettlementLine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReportDigitalSign",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    SignerUserId = table.Column<long>(type: "bigint", nullable: false),
                    SignedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignatureReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_ReportDigitalSign", x => x.Id);
                    table.UniqueConstraint("AK_LMS_ReportDigitalSign_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReportPaymentGate",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    LabInvoiceHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    MinimumPaidPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsReleased = table.Column<bool>(type: "bit", nullable: false),
                    ReleasedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleaseReasonReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LMS_ReportPaymentGate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_ReportValidationStep",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: false),
                    ValidationLevel = table.Column<int>(type: "int", nullable: false),
                    ValidatorUserId = table.Column<long>(type: "bigint", nullable: false),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ValidatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_ReportValidationStep", x => x.Id);
                    table.UniqueConstraint("AK_LMS_ReportValidationStep_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LMS_ResultDeltaCheck",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentLabResultId = table.Column<long>(type: "bigint", nullable: false),
                    PriorLabResultId = table.Column<long>(type: "bigint", nullable: false),
                    DeltaPercent = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Flagged = table.Column<bool>(type: "bit", nullable: false),
                    EvaluatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_ResultDeltaCheck", x => x.Id);
                    table.UniqueConstraint("AK_LMS_ResultDeltaCheck_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LMS_TestPackage",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PackageName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_TestPackage", x => x.Id);
                    table.UniqueConstraint("AK_LMS_TestPackage_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "LMS_TestPackageLine",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestPackageId = table.Column<long>(type: "bigint", nullable: false),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    IsOptionalInPackage = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_LMS_TestPackageLine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_TestPrice",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: true),
                    TestPackageId = table.Column<long>(type: "bigint", nullable: true),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    PriceTierReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    RateAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_LMS_TestPrice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LMS_TestReagentMap",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestMasterId = table.Column<long>(type: "bigint", nullable: false),
                    ReagentMasterId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityPerTest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LMS_TestReagentMap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturer",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ManufacturerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
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
                    table.PrimaryKey("PK_Manufacturer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medicine",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    MedicineName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ManufacturerId = table.Column<long>(type: "bigint", nullable: true),
                    Strength = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    DefaultUnitId = table.Column<long>(type: "bigint", nullable: true),
                    FormReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_Medicine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicineBatch",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineId = table.Column<long>(type: "bigint", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    MRP = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    PurchaseRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ManufacturingDate = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_MedicineBatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicineCategory",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_MedicineCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicineComposition",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineId = table.Column<long>(type: "bigint", nullable: false),
                    CompositionId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_MedicineComposition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_BatchStockLocation",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchStockId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryLocationId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityOnHand = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Pharmacy_BatchStockLocation", x => x.Id);
                    table.UniqueConstraint("AK_Pharmacy_BatchStockLocation_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_ControlledDrugRegister",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacySalesItemId = table.Column<long>(type: "bigint", nullable: false),
                    PrescribingDoctorId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    PatientAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    PatientAcknowledgedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisterEntryOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Pharmacy_ControlledDrugRegister", x => x.Id);
                    table.UniqueConstraint("AK_Pharmacy_ControlledDrugRegister_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_InventoryLocation",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LocationTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ParentLocationId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Pharmacy_InventoryLocation", x => x.Id);
                    table.UniqueConstraint("AK_Pharmacy_InventoryLocation_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_Pharmacy_InventoryLocation_Pharmacy_InventoryLocation_TenantId_FacilityId_ParentLocationId",
                        columns: x => new { x.TenantId, x.FacilityId, x.ParentLocationId },
                        principalSchema: "pharmacy",
                        principalTable: "Pharmacy_InventoryLocation",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_ReorderPolicy",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchStockId = table.Column<long>(type: "bigint", nullable: false),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    EconomicOrderQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Pharmacy_ReorderPolicy", x => x.Id);
                    table.UniqueConstraint("AK_Pharmacy_ReorderPolicy_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_SalesReturn",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    OriginalSalesId = table.Column<long>(type: "bigint", nullable: false),
                    ReturnReasonReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ReturnedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Pharmacy_SalesReturn", x => x.Id);
                    table.UniqueConstraint("AK_Pharmacy_SalesReturn_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "PharmacySales",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    VisitId = table.Column<long>(type: "bigint", nullable: true),
                    DoctorId = table.Column<long>(type: "bigint", nullable: true),
                    SalesDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    PaymentTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                    table.PrimaryKey("PK_PharmacySales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PharmacySalesItems",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacySalesId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<long>(type: "bigint", nullable: false),
                    MedicineBatchId = table.Column<long>(type: "bigint", nullable: false),
                    QuantitySold = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    LineTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DispensedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_PharmacySalesItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionMapping",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<long>(type: "bigint", nullable: false),
                    PharmacySalesId = table.Column<long>(type: "bigint", nullable: false),
                    PrescriptionItemId = table.Column<long>(type: "bigint", nullable: true),
                    PharmacySalesItemId = table.Column<long>(type: "bigint", nullable: true),
                    MappedQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MappingNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_PrescriptionMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingStages",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StageCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    StageNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_ProcessingStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrder",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_PurchaseOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityOrdered = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    PurchaseRate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QCRecords",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QCTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    QCStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    LotNo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
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
                    table.PrimaryKey("PK_QCRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QCResults",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QCRecordId = table.Column<long>(type: "bigint", nullable: false),
                    TestParameterId = table.Column<long>(type: "bigint", nullable: true),
                    ResultNumeric = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ResultText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultUnitId = table.Column<long>(type: "bigint", nullable: true),
                    IsPass = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_QCResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SEC_DataChangeAuditLog",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    ActionTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    EntitySchema = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EntityKeyJson = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangeSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ClientIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_SEC_DataChangeAuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustment",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjustmentNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AdjustmentOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdjustmentTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    PerformedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    ReasonNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_StockAdjustment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustmentItems",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockAdjustmentId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    MedicineBatchId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityDelta = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
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
                    table.PrimaryKey("PK_StockAdjustmentItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockLedger",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineBatchId = table.Column<long>(type: "bigint", nullable: false),
                    LedgerTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    TransactionOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityDelta = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    BeforeQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AfterQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SourceReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_StockLedger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockTransfer",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FromFacilityId = table.Column<long>(type: "bigint", nullable: false),
                    ToFacilityId = table.Column<long>(type: "bigint", nullable: false),
                    TransferOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_StockTransfer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockTransferItems",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockTransferId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    MedicineBatchId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
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
                    table.PrimaryKey("PK_StockTransferItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianAssignment",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkQueueId = table.Column<long>(type: "bigint", nullable: false),
                    TechnicianDoctorId = table.Column<long>(type: "bigint", nullable: false),
                    AssignmentStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReleasedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_TechnicianAssignment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkQueue",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    StageId = table.Column<long>(type: "bigint", nullable: false),
                    PriorityReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    QueueStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    QueuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClaimedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedByDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    AssignedTechnicianDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    QueueNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_WorkQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COM_NotificationQueue",
                schema: "communication",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_COM_NotificationQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_COM_NotificationQueue_COM_Notification_TenantId_FacilityId_NotificationId",
                        columns: x => new { x.TenantId, x.FacilityId, x.NotificationId },
                        principalSchema: "communication",
                        principalTable: "COM_Notification",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "COM_NotificationRecipient",
                schema: "communication",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationId = table.Column<long>(type: "bigint", nullable: false),
                    RecipientTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    RecipientId = table.Column<long>(type: "bigint", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_COM_NotificationRecipient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_COM_NotificationRecipient_COM_Notification_TenantId_FacilityId_NotificationId",
                        columns: x => new { x.TenantId, x.FacilityId, x.NotificationId },
                        principalSchema: "communication",
                        principalTable: "COM_Notification",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "COM_NotificationChannel",
                schema: "communication",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationId = table.Column<long>(type: "bigint", nullable: false),
                    ChannelTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateId = table.Column<long>(type: "bigint", nullable: true),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    LastAttemptOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_COM_NotificationChannel", x => x.Id);
                    table.UniqueConstraint("AK_COM_NotificationChannel_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_COM_NotificationChannel_COM_NotificationTemplate_TenantId_FacilityId_TemplateId",
                        columns: x => new { x.TenantId, x.FacilityId, x.TemplateId },
                        principalSchema: "communication",
                        principalTable: "COM_NotificationTemplate",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                    table.ForeignKey(
                        name: "FK_COM_NotificationChannel_COM_Notification_TenantId_FacilityId_NotificationId",
                        columns: x => new { x.TenantId, x.FacilityId, x.NotificationId },
                        principalSchema: "communication",
                        principalTable: "COM_Notification",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enterprise",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    EnterpriseCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EnterpriseName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    RegistrationDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlobalSettingsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryAddressId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryContactId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprise", x => x.Id);
                    table.UniqueConstraint("AK_Enterprise_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Enterprise_Address_TenantId_PrimaryAddressId",
                        columns: x => new { x.TenantId, x.PrimaryAddressId },
                        principalSchema: "shared",
                        principalTable: "Address",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_Enterprise_ContactDetails_TenantId_PrimaryContactId",
                        columns: x => new { x.TenantId, x.PrimaryContactId },
                        principalSchema: "shared",
                        principalTable: "ContactDetails",
                        principalColumns: new[] { "TenantId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "LMS_EquipmentFacilityMapping",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentFacilityId = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false),
                    MappedFacilityId = table.Column<long>(type: "bigint", nullable: false),
                    MappingNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_LMS_EquipmentFacilityMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LMS_EquipmentFacilityMapping_Equipment_TenantId_EquipmentFacilityId_EquipmentId",
                        columns: x => new { x.TenantId, x.EquipmentFacilityId, x.EquipmentId },
                        principalSchema: "lms",
                        principalTable: "Equipment",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_PackageDefinitionLine",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ServiceCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_PackageDefinitionLine", x => x.Id);
                    table.UniqueConstraint("AK_HMS_PackageDefinitionLine_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_PackageDefinitionLine_HMS_PackageDefinition_TenantId_FacilityId_PackageDefinitionId",
                        columns: x => new { x.TenantId, x.FacilityId, x.PackageDefinitionId },
                        principalSchema: "hms",
                        principalTable: "HMS_PackageDefinition",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_Claim",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    InsuranceProviderId = table.Column<long>(type: "bigint", nullable: false),
                    PatientMasterId = table.Column<long>(type: "bigint", nullable: false),
                    BillingHeaderId = table.Column<long>(type: "bigint", nullable: true),
                    SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_Claim", x => x.Id);
                    table.UniqueConstraint("AK_HMS_Claim_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_Claim_HMS_BillingHeader_TenantId_FacilityId_BillingHeaderId",
                        columns: x => new { x.TenantId, x.FacilityId, x.BillingHeaderId },
                        principalSchema: "hms",
                        principalTable: "HMS_BillingHeader",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                    table.ForeignKey(
                        name: "FK_HMS_Claim_HMS_InsuranceProvider_TenantId_InsuranceProviderId",
                        columns: x => new { x.TenantId, x.InsuranceProviderId },
                        principalSchema: "hms",
                        principalTable: "HMS_InsuranceProvider",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HMS_Claim_HMS_PatientMaster_TenantId_PatientMasterId",
                        columns: x => new { x.TenantId, x.PatientMasterId },
                        principalSchema: "hms",
                        principalTable: "HMS_PatientMaster",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_PatientFacilityLink",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientMasterId = table.Column<long>(type: "bigint", nullable: false),
                    LinkedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_HMS_PatientFacilityLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HMS_PatientFacilityLink_HMS_PatientMaster_TenantId_PatientMasterId",
                        columns: x => new { x.TenantId, x.PatientMasterId },
                        principalSchema: "hms",
                        principalTable: "HMS_PatientMaster",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_PreAuthorization",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreAuthNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    InsuranceProviderId = table.Column<long>(type: "bigint", nullable: false),
                    PatientMasterId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_PreAuthorization", x => x.Id);
                    table.UniqueConstraint("AK_HMS_PreAuthorization_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_PreAuthorization_HMS_InsuranceProvider_TenantId_InsuranceProviderId",
                        columns: x => new { x.TenantId, x.InsuranceProviderId },
                        principalSchema: "hms",
                        principalTable: "HMS_InsuranceProvider",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HMS_PreAuthorization_HMS_PatientMaster_TenantId_PatientMasterId",
                        columns: x => new { x.TenantId, x.PatientMasterId },
                        principalSchema: "hms",
                        principalTable: "HMS_PatientMaster",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_SurgerySchedule",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationTheatreId = table.Column<long>(type: "bigint", nullable: false),
                    PatientMasterId = table.Column<long>(type: "bigint", nullable: false),
                    SurgeonDoctorId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledStartOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledEndOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcedureSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ScheduleStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_SurgerySchedule", x => x.Id);
                    table.UniqueConstraint("AK_HMS_SurgerySchedule_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_SurgerySchedule_HMS_OperationTheatre_TenantId_FacilityId_OperationTheatreId",
                        columns: x => new { x.TenantId, x.FacilityId, x.OperationTheatreId },
                        principalSchema: "hms",
                        principalTable: "HMS_OperationTheatre",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HMS_SurgerySchedule_HMS_PatientMaster_TenantId_PatientMasterId",
                        columns: x => new { x.TenantId, x.PatientMasterId },
                        principalSchema: "hms",
                        principalTable: "HMS_PatientMaster",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_Appointment",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    DoctorId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    VisitTypeId = table.Column<long>(type: "bigint", nullable: true),
                    AppointmentStatusValueId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledStartOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledEndOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PriorityReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_Appointment", x => x.Id);
                    table.UniqueConstraint("AK_HMS_Appointment_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_Appointment_HMS_VisitType_TenantId_VisitTypeId",
                        columns: x => new { x.TenantId, x.VisitTypeId },
                        principalSchema: "hms",
                        principalTable: "HMS_VisitType",
                        principalColumns: new[] { "TenantId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "HMS_Bed",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WardId = table.Column<long>(type: "bigint", nullable: false),
                    BedCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    BedCategoryReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    BedOperationalStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    CurrentAdmissionId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_Bed", x => x.Id);
                    table.UniqueConstraint("AK_HMS_Bed_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_Bed_HMS_Ward_TenantId_FacilityId_WardId",
                        columns: x => new { x.TenantId, x.FacilityId, x.WardId },
                        principalSchema: "hms",
                        principalTable: "HMS_Ward",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity_RolePermission",
                schema: "identity",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Identity_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_Identity_RolePermission_Identity_Permission_TenantId_PermissionId",
                        columns: x => new { x.TenantId, x.PermissionId },
                        principalSchema: "identity",
                        principalTable: "Identity_Permission",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Identity_RolePermission_Identity_Role_TenantId_RoleId",
                        columns: x => new { x.TenantId, x.RoleId },
                        principalSchema: "identity",
                        principalTable: "Identity_Role",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity_AccountLockoutState",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    FailedAttemptCount = table.Column<int>(type: "int", nullable: false),
                    LockoutEndOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastFailedAttemptOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_AccountLockoutState", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Identity_AccountLockoutState_Identity_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Identity_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity_LoginAttempt",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    EmailAttempted = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    FailureReasonReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_LoginAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity_LoginAttempt_Identity_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Identity_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Identity_PasswordResetToken",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestChannelReferenceValueId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_PasswordResetToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity_PasswordResetToken_Identity_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Identity_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity_RefreshToken",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TokenFamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByTokenId = table.Column<long>(type: "bigint", nullable: true),
                    ClientIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity_RefreshToken_Identity_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Identity_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity_UserFacilityGrant",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GrantFacilityId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_UserFacilityGrant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity_UserFacilityGrant_Identity_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Identity_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity_UserProfile",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreferredLocaleReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MfaEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity_UserProfile", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Identity_UserProfile_Identity_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Identity_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identity_UserRole",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Identity_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Identity_UserRole_Identity_Role_TenantId_RoleId",
                        columns: x => new { x.TenantId, x.RoleId },
                        principalSchema: "identity",
                        principalTable: "Identity_Role",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Identity_UserRole_Identity_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Identity_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LIS_AnalyzerResultLine",
                schema: "lis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalyzerResultHeaderId = table.Column<long>(type: "bigint", nullable: false),
                    LmsCatalogParameterId = table.Column<long>(type: "bigint", nullable: true),
                    EquipmentResultCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ResultNumeric = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ResultText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResultUnitId = table.Column<long>(type: "bigint", nullable: true),
                    LineStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LIS_AnalyzerResultLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LIS_AnalyzerResultLine_LIS_AnalyzerResultHeader_TenantId_FacilityId_AnalyzerResultHeaderId",
                        columns: x => new { x.TenantId, x.FacilityId, x.AnalyzerResultHeaderId },
                        principalSchema: "lis",
                        principalTable: "LIS_AnalyzerResultHeader",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "LMS_CatalogReferenceRange",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogParameterId = table.Column<long>(type: "bigint", nullable: false),
                    SexReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    AgeFromYears = table.Column<int>(type: "int", nullable: true),
                    AgeToYears = table.Column<int>(type: "int", nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RangeText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RangeNotes = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
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
                    table.PrimaryKey("PK_LMS_CatalogReferenceRange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LMS_CatalogReferenceRange_LMS_CatalogParameter_TenantId_CatalogParameterId",
                        columns: x => new { x.TenantId, x.CatalogParameterId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogParameter",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LMS_CatalogTestEquipmentMap",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogTestId = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_LMS_CatalogTestEquipmentMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LMS_CatalogTestEquipmentMap_Equipment_TenantId_FacilityId_EquipmentId",
                        columns: x => new { x.TenantId, x.FacilityId, x.EquipmentId },
                        principalSchema: "lms",
                        principalTable: "Equipment",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                    table.ForeignKey(
                        name: "FK_LMS_CatalogTestEquipmentMap_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId",
                        columns: x => new { x.TenantId, x.FacilityId, x.CatalogTestId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogTest",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "LMS_CatalogTestParameterMap",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogTestId = table.Column<long>(type: "bigint", nullable: false),
                    CatalogParameterId = table.Column<long>(type: "bigint", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LMS_CatalogTestParameterMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LMS_CatalogTestParameterMap_LMS_CatalogParameter_TenantId_CatalogParameterId",
                        columns: x => new { x.TenantId, x.CatalogParameterId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogParameter",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LMS_CatalogTestParameterMap_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId",
                        columns: x => new { x.TenantId, x.FacilityId, x.CatalogTestId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogTest",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "LMS_EquipmentTestMaster",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false),
                    CatalogTestId = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentAssayCode = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EquipmentAssayName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
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
                    table.PrimaryKey("PK_LMS_EquipmentTestMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LMS_EquipmentTestMaster_Equipment_TenantId_FacilityId_EquipmentId",
                        columns: x => new { x.TenantId, x.FacilityId, x.EquipmentId },
                        principalSchema: "lms",
                        principalTable: "Equipment",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                    table.ForeignKey(
                        name: "FK_LMS_EquipmentTestMaster_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId",
                        columns: x => new { x.TenantId, x.FacilityId, x.CatalogTestId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogTest",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "LMS_RiderTracking",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionRequestId = table.Column<long>(type: "bigint", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_RiderTracking", x => x.Id);
                    table.UniqueConstraint("AK_LMS_RiderTracking_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_LMS_RiderTracking_LMS_CollectionRequest_TenantId_FacilityId_CollectionRequestId",
                        columns: x => new { x.TenantId, x.FacilityId, x.CollectionRequestId },
                        principalSchema: "lms",
                        principalTable: "LMS_CollectionRequest",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LMS_SampleTransport",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionRequestId = table.Column<long>(type: "bigint", nullable: false),
                    TemperatureCelsius = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_SampleTransport", x => x.Id);
                    table.UniqueConstraint("AK_LMS_SampleTransport_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_LMS_SampleTransport_LMS_CollectionRequest_TenantId_FacilityId_CollectionRequestId",
                        columns: x => new { x.TenantId, x.FacilityId, x.CollectionRequestId },
                        principalSchema: "lms",
                        principalTable: "LMS_CollectionRequest",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LMS_LabTestBookingItem",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestBookingId = table.Column<long>(type: "bigint", nullable: false),
                    CatalogTestId = table.Column<long>(type: "bigint", nullable: false),
                    WorkflowStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    LineNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_LabTestBookingItem", x => x.Id);
                    table.UniqueConstraint("AK_LMS_LabTestBookingItem_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_LMS_LabTestBookingItem_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId",
                        columns: x => new { x.TenantId, x.FacilityId, x.CatalogTestId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogTest",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LMS_LabTestBookingItem_LMS_LabTestBooking_TenantId_FacilityId_LabTestBookingId",
                        columns: x => new { x.TenantId, x.FacilityId, x.LabTestBookingId },
                        principalSchema: "lms",
                        principalTable: "LMS_LabTestBooking",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LMS_CatalogPackageParameterMap",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestPackageId = table.Column<long>(type: "bigint", nullable: false),
                    CatalogParameterId = table.Column<long>(type: "bigint", nullable: false),
                    CatalogTestId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_LMS_CatalogPackageParameterMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LMS_CatalogPackageParameterMap_LMS_CatalogParameter_TenantId_CatalogParameterId",
                        columns: x => new { x.TenantId, x.CatalogParameterId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogParameter",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LMS_CatalogPackageParameterMap_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId",
                        columns: x => new { x.TenantId, x.FacilityId, x.CatalogTestId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogTest",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                    table.ForeignKey(
                        name: "FK_LMS_CatalogPackageParameterMap_LMS_TestPackage_TenantId_FacilityId_TestPackageId",
                        columns: x => new { x.TenantId, x.FacilityId, x.TestPackageId },
                        principalSchema: "lms",
                        principalTable: "LMS_TestPackage",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "LMS_CatalogPackageTestLineMap",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestPackageId = table.Column<long>(type: "bigint", nullable: false),
                    LineNum = table.Column<int>(type: "int", nullable: false),
                    CatalogTestId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_LMS_CatalogPackageTestLineMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LMS_CatalogPackageTestLineMap_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId",
                        columns: x => new { x.TenantId, x.FacilityId, x.CatalogTestId },
                        principalSchema: "lms",
                        principalTable: "LMS_CatalogTest",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                    table.ForeignKey(
                        name: "FK_LMS_CatalogPackageTestLineMap_LMS_TestPackage_TenantId_FacilityId_TestPackageId",
                        columns: x => new { x.TenantId, x.FacilityId, x.TestPackageId },
                        principalSchema: "lms",
                        principalTable: "LMS_TestPackage",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy_SalesReturnItem",
                schema: "pharmacy",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesReturnId = table.Column<long>(type: "bigint", nullable: false),
                    OriginalSalesItemId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityReturned = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ReconciliationStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Pharmacy_SalesReturnItem", x => x.Id);
                    table.UniqueConstraint("AK_Pharmacy_SalesReturnItem_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_Pharmacy_SalesReturnItem_Pharmacy_SalesReturn_TenantId_FacilityId_SalesReturnId",
                        columns: x => new { x.TenantId, x.FacilityId, x.SalesReturnId },
                        principalSchema: "pharmacy",
                        principalTable: "Pharmacy_SalesReturn",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "COM_NotificationLog",
                schema: "communication",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationChannelId = table.Column<long>(type: "bigint", nullable: false),
                    AttemptNo = table.Column<int>(type: "int", nullable: false),
                    RequestPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsePayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_COM_NotificationLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_COM_NotificationLog_COM_NotificationChannel_TenantId_FacilityId_NotificationChannelId",
                        columns: x => new { x.TenantId, x.FacilityId, x.NotificationChannelId },
                        principalSchema: "communication",
                        principalTable: "COM_NotificationChannel",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    EnterpriseId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GSTIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalIdentifier1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalIdentifier2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryAddressId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryContactId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.UniqueConstraint("AK_Company_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Company_Address_TenantId_PrimaryAddressId",
                        columns: x => new { x.TenantId, x.PrimaryAddressId },
                        principalSchema: "shared",
                        principalTable: "Address",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_Company_ContactDetails_TenantId_PrimaryContactId",
                        columns: x => new { x.TenantId, x.PrimaryContactId },
                        principalSchema: "shared",
                        principalTable: "ContactDetails",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_Company_Enterprise_TenantId_EnterpriseId",
                        columns: x => new { x.TenantId, x.EnterpriseId },
                        principalSchema: "shared",
                        principalTable: "Enterprise",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_AnesthesiaRecord",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurgeryScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    AnesthesiologistDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    RecordJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_AnesthesiaRecord", x => x.Id);
                    table.UniqueConstraint("AK_HMS_AnesthesiaRecord_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_AnesthesiaRecord_HMS_SurgerySchedule_TenantId_FacilityId_SurgeryScheduleId",
                        columns: x => new { x.TenantId, x.FacilityId, x.SurgeryScheduleId },
                        principalSchema: "hms",
                        principalTable: "HMS_SurgerySchedule",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_OTConsumable",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurgeryScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Billable = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_OTConsumable", x => x.Id);
                    table.UniqueConstraint("AK_HMS_OTConsumable_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_OTConsumable_HMS_SurgerySchedule_TenantId_FacilityId_SurgeryScheduleId",
                        columns: x => new { x.TenantId, x.FacilityId, x.SurgeryScheduleId },
                        principalSchema: "hms",
                        principalTable: "HMS_SurgerySchedule",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_PostOpRecord",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurgeryScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    RecoveryNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_PostOpRecord", x => x.Id);
                    table.UniqueConstraint("AK_HMS_PostOpRecord_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_PostOpRecord_HMS_SurgerySchedule_TenantId_FacilityId_SurgeryScheduleId",
                        columns: x => new { x.TenantId, x.FacilityId, x.SurgeryScheduleId },
                        principalSchema: "hms",
                        principalTable: "HMS_SurgerySchedule",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_Visit",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: true),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    DoctorId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    VisitTypeId = table.Column<long>(type: "bigint", nullable: false),
                    VisitStartOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitEndOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChiefComplaint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CurrentStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_Visit", x => x.Id);
                    table.UniqueConstraint("AK_HMS_Visit_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_Visit_HMS_Appointment_TenantId_FacilityId_AppointmentId",
                        columns: x => new { x.TenantId, x.FacilityId, x.AppointmentId },
                        principalSchema: "hms",
                        principalTable: "HMS_Appointment",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                    table.ForeignKey(
                        name: "FK_HMS_Visit_HMS_VisitType_TenantId_VisitTypeId",
                        columns: x => new { x.TenantId, x.VisitTypeId },
                        principalSchema: "hms",
                        principalTable: "HMS_VisitType",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_Admission",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PatientMasterId = table.Column<long>(type: "bigint", nullable: false),
                    BedId = table.Column<long>(type: "bigint", nullable: false),
                    AdmissionStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    AdmittedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DischargedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttendingDoctorId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_Admission", x => x.Id);
                    table.UniqueConstraint("AK_HMS_Admission_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_Admission_HMS_Bed_TenantId_FacilityId_BedId",
                        columns: x => new { x.TenantId, x.FacilityId, x.BedId },
                        principalSchema: "hms",
                        principalTable: "HMS_Bed",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HMS_Admission_HMS_PatientMaster_TenantId_PatientMasterId",
                        columns: x => new { x.TenantId, x.PatientMasterId },
                        principalSchema: "hms",
                        principalTable: "HMS_PatientMaster",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HMS_HousekeepingStatus",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BedId = table.Column<long>(type: "bigint", nullable: false),
                    HousekeepingStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    RecordedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_HMS_HousekeepingStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HMS_HousekeepingStatus_HMS_Bed_TenantId_FacilityId_BedId",
                        columns: x => new { x.TenantId, x.FacilityId, x.BedId },
                        principalSchema: "hms",
                        principalTable: "HMS_Bed",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "LMS_LabSampleBarcode",
                schema: "lms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarcodeValue = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TestBookingItemId = table.Column<long>(type: "bigint", nullable: false),
                    SampleTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: true),
                    BarcodeStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    RegisteredFromSystem = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
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
                    table.PrimaryKey("PK_LMS_LabSampleBarcode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LMS_LabSampleBarcode_LMS_LabTestBookingItem_TenantId_FacilityId_TestBookingItemId",
                        columns: x => new { x.TenantId, x.FacilityId, x.TestBookingItemId },
                        principalSchema: "lms",
                        principalTable: "LMS_LabTestBookingItem",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "BusinessUnit",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessUnitCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BusinessUnitName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BusinessUnitType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryAddressId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryContactId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnit", x => x.Id);
                    table.UniqueConstraint("AK_BusinessUnit_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_BusinessUnit_Address_TenantId_PrimaryAddressId",
                        columns: x => new { x.TenantId, x.PrimaryAddressId },
                        principalSchema: "shared",
                        principalTable: "Address",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_BusinessUnit_Company_TenantId_CompanyId",
                        columns: x => new { x.TenantId, x.CompanyId },
                        principalSchema: "shared",
                        principalTable: "Company",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessUnit_ContactDetails_TenantId_PrimaryContactId",
                        columns: x => new { x.TenantId, x.PrimaryContactId },
                        principalSchema: "shared",
                        principalTable: "ContactDetails",
                        principalColumns: new[] { "TenantId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "HMS_ProformaInvoice",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProformaNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PatientMasterId = table.Column<long>(type: "bigint", nullable: true),
                    VisitId = table.Column<long>(type: "bigint", nullable: true),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    StatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    LinesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_HMS_ProformaInvoice", x => x.Id);
                    table.UniqueConstraint("AK_HMS_ProformaInvoice_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_HMS_ProformaInvoice_HMS_PatientMaster_TenantId_PatientMasterId",
                        columns: x => new { x.TenantId, x.PatientMasterId },
                        principalSchema: "hms",
                        principalTable: "HMS_PatientMaster",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_HMS_ProformaInvoice_HMS_Visit_TenantId_FacilityId_VisitId",
                        columns: x => new { x.TenantId, x.FacilityId, x.VisitId },
                        principalSchema: "hms",
                        principalTable: "HMS_Visit",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "HMS_AdmissionTransfer",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: false),
                    FromBedId = table.Column<long>(type: "bigint", nullable: false),
                    ToBedId = table.Column<long>(type: "bigint", nullable: false),
                    TransferredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_HMS_AdmissionTransfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HMS_AdmissionTransfer_HMS_Admission_TenantId_FacilityId_AdmissionId",
                        columns: x => new { x.TenantId, x.FacilityId, x.AdmissionId },
                        principalSchema: "hms",
                        principalTable: "HMS_Admission",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "HMS_DoctorOrderAlert",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<long>(type: "bigint", nullable: true),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: true),
                    DoctorId = table.Column<long>(type: "bigint", nullable: false),
                    AlertTypeReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AcknowledgedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_HMS_DoctorOrderAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HMS_DoctorOrderAlert_HMS_Admission_TenantId_FacilityId_AdmissionId",
                        columns: x => new { x.TenantId, x.FacilityId, x.AdmissionId },
                        principalSchema: "hms",
                        principalTable: "HMS_Admission",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                    table.ForeignKey(
                        name: "FK_HMS_DoctorOrderAlert_HMS_Visit_TenantId_FacilityId_VisitId",
                        columns: x => new { x.TenantId, x.FacilityId, x.VisitId },
                        principalSchema: "hms",
                        principalTable: "HMS_Visit",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "HMS_EmarEntry",
                schema: "hms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: false),
                    MedicationCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ScheduledOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministeredOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdministrationStatusReferenceValueId = table.Column<long>(type: "bigint", nullable: false),
                    NurseUserId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_HMS_EmarEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HMS_EmarEntry_HMS_Admission_TenantId_FacilityId_AdmissionId",
                        columns: x => new { x.TenantId, x.FacilityId, x.AdmissionId },
                        principalSchema: "hms",
                        principalTable: "HMS_Admission",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    BusinessUnitId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FacilityName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FacilityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeoCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryAddressId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryContactId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                    table.UniqueConstraint("AK_Facility_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Facility_Address_TenantId_PrimaryAddressId",
                        columns: x => new { x.TenantId, x.PrimaryAddressId },
                        principalSchema: "shared",
                        principalTable: "Address",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_Facility_BusinessUnit_TenantId_BusinessUnitId",
                        columns: x => new { x.TenantId, x.BusinessUnitId },
                        principalSchema: "shared",
                        principalTable: "BusinessUnit",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Facility_ContactDetails_TenantId_PrimaryContactId",
                        columns: x => new { x.TenantId, x.PrimaryContactId },
                        principalSchema: "shared",
                        principalTable: "ContactDetails",
                        principalColumns: new[] { "TenantId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "Department",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityParentId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DepartmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParentDepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryAddressId = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryContactId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.UniqueConstraint("AK_Department_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Department_Address_TenantId_PrimaryAddressId",
                        columns: x => new { x.TenantId, x.PrimaryAddressId },
                        principalSchema: "shared",
                        principalTable: "Address",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_Department_ContactDetails_TenantId_PrimaryContactId",
                        columns: x => new { x.TenantId, x.PrimaryContactId },
                        principalSchema: "shared",
                        principalTable: "ContactDetails",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_Department_Department_TenantId_ParentDepartmentId",
                        columns: x => new { x.TenantId, x.ParentDepartmentId },
                        principalSchema: "shared",
                        principalTable: "Department",
                        principalColumns: new[] { "TenantId", "Id" });
                    table.ForeignKey(
                        name: "FK_Department_Facility_TenantId_FacilityParentId",
                        columns: x => new { x.TenantId, x.FacilityParentId },
                        principalSchema: "shared",
                        principalTable: "Facility",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EXT_CrossFacilityReportAudit",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    ReportCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ReportName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FacilityScopeJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilterJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultRowCount = table.Column<int>(type: "int", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXT_CrossFacilityReportAudit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXT_CrossFacilityReportAudit_Facility_TenantId_FacilityId",
                        columns: x => new { x.TenantId, x.FacilityId },
                        principalSchema: "shared",
                        principalTable: "Facility",
                        principalColumns: new[] { "TenantId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "EXT_EnterpriseB2BContract",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    EnterpriseId = table.Column<long>(type: "bigint", nullable: false),
                    PartnerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PartnerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContractCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TermsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXT_EnterpriseB2BContract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXT_EnterpriseB2BContract_Enterprise_TenantId_EnterpriseId",
                        columns: x => new { x.TenantId, x.EnterpriseId },
                        principalSchema: "shared",
                        principalTable: "Enterprise",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EXT_EnterpriseB2BContract_Facility_TenantId_FacilityId",
                        columns: x => new { x.TenantId, x.FacilityId },
                        principalSchema: "shared",
                        principalTable: "Facility",
                        principalColumns: new[] { "TenantId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "EXT_FacilityServicePriceList",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
                    PriceListCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PriceListName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ServiceModule = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PartnerReferenceCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXT_FacilityServicePriceList", x => x.Id);
                    table.UniqueConstraint("AK_EXT_FacilityServicePriceList_TenantId_FacilityId_Id", x => new { x.TenantId, x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_EXT_FacilityServicePriceList_Facility_TenantId_FacilityId",
                        columns: x => new { x.TenantId, x.FacilityId },
                        principalSchema: "shared",
                        principalTable: "Facility",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EXT_LabCriticalValueEscalation",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: true),
                    LabOrderItemId = table.Column<long>(type: "bigint", nullable: true),
                    LabResultId = table.Column<long>(type: "bigint", nullable: true),
                    EscalationLevel = table.Column<int>(type: "int", nullable: false),
                    ChannelCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RecipientSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DispatchedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcknowledgedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OutcomeCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXT_LabCriticalValueEscalation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXT_LabCriticalValueEscalation_Facility_TenantId_FacilityId",
                        columns: x => new { x.TenantId, x.FacilityId },
                        principalSchema: "shared",
                        principalTable: "Facility",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EXT_ModuleIntegrationHandoff",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SourceModule = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TargetModule = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SourceEntityId = table.Column<long>(type: "bigint", nullable: true),
                    TargetEntityId = table.Column<long>(type: "bigint", nullable: true),
                    StatusCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DetailJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXT_ModuleIntegrationHandoff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXT_ModuleIntegrationHandoff_Facility_TenantId_FacilityId",
                        columns: x => new { x.TenantId, x.FacilityId },
                        principalSchema: "shared",
                        principalTable: "Facility",
                        principalColumns: new[] { "TenantId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "EXT_TenantOnboardingStage",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: true),
                    StageCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    StageStatus = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXT_TenantOnboardingStage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXT_TenantOnboardingStage_Facility_TenantId_FacilityId",
                        columns: x => new { x.TenantId, x.FacilityId },
                        principalSchema: "shared",
                        principalTable: "Facility",
                        principalColumns: new[] { "TenantId", "Id" });
                });

            migrationBuilder.CreateTable(
                name: "EXT_FacilityServicePriceListLine",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
                    PriceListId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceItemCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ServiceItemName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TaxCategoryCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXT_FacilityServicePriceListLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EXT_FacilityServicePriceListLine_EXT_FacilityServicePriceList_TenantId_FacilityId_PriceListId",
                        columns: x => new { x.TenantId, x.FacilityId, x.PriceListId },
                        principalSchema: "shared",
                        principalTable: "EXT_FacilityServicePriceList",
                        principalColumns: new[] { "TenantId", "FacilityId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_TenantId_BusinessUnitCode",
                schema: "shared",
                table: "BusinessUnit",
                columns: new[] { "TenantId", "BusinessUnitCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_TenantId_CompanyId",
                schema: "shared",
                table: "BusinessUnit",
                columns: new[] { "TenantId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_TenantId_PrimaryAddressId",
                schema: "shared",
                table: "BusinessUnit",
                columns: new[] { "TenantId", "PrimaryAddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_TenantId_PrimaryContactId",
                schema: "shared",
                table: "BusinessUnit",
                columns: new[] { "TenantId", "PrimaryContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_COM_NotificationChannel_TenantId_FacilityId_NotificationId",
                schema: "communication",
                table: "COM_NotificationChannel",
                columns: new[] { "TenantId", "FacilityId", "NotificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_COM_NotificationChannel_TenantId_FacilityId_TemplateId",
                schema: "communication",
                table: "COM_NotificationChannel",
                columns: new[] { "TenantId", "FacilityId", "TemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_COM_NotificationLog_TenantId_FacilityId_NotificationChannelId",
                schema: "communication",
                table: "COM_NotificationLog",
                columns: new[] { "TenantId", "FacilityId", "NotificationChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_COM_NotificationQueue_TenantId_FacilityId_NotificationId",
                schema: "communication",
                table: "COM_NotificationQueue",
                columns: new[] { "TenantId", "FacilityId", "NotificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_COM_NotificationRecipient_TenantId_FacilityId_NotificationId",
                schema: "communication",
                table: "COM_NotificationRecipient",
                columns: new[] { "TenantId", "FacilityId", "NotificationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Company_TenantId_CompanyCode",
                schema: "shared",
                table: "Company",
                columns: new[] { "TenantId", "CompanyCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_TenantId_EnterpriseId",
                schema: "shared",
                table: "Company",
                columns: new[] { "TenantId", "EnterpriseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Company_TenantId_PrimaryAddressId",
                schema: "shared",
                table: "Company",
                columns: new[] { "TenantId", "PrimaryAddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_Company_TenantId_PrimaryContactId",
                schema: "shared",
                table: "Company",
                columns: new[] { "TenantId", "PrimaryContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_Department_TenantId_DepartmentCode",
                schema: "shared",
                table: "Department",
                columns: new[] { "TenantId", "DepartmentCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_TenantId_FacilityParentId",
                schema: "shared",
                table: "Department",
                columns: new[] { "TenantId", "FacilityParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Department_TenantId_ParentDepartmentId",
                schema: "shared",
                table: "Department",
                columns: new[] { "TenantId", "ParentDepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Department_TenantId_PrimaryAddressId",
                schema: "shared",
                table: "Department",
                columns: new[] { "TenantId", "PrimaryAddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_Department_TenantId_PrimaryContactId",
                schema: "shared",
                table: "Department",
                columns: new[] { "TenantId", "PrimaryContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_Enterprise_TenantId_EnterpriseCode",
                schema: "shared",
                table: "Enterprise",
                columns: new[] { "TenantId", "EnterpriseCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enterprise_TenantId_PrimaryAddressId",
                schema: "shared",
                table: "Enterprise",
                columns: new[] { "TenantId", "PrimaryAddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_Enterprise_TenantId_PrimaryContactId",
                schema: "shared",
                table: "Enterprise",
                columns: new[] { "TenantId", "PrimaryContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_EXT_CrossFacilityReportAudit_TenantId_FacilityId",
                schema: "shared",
                table: "EXT_CrossFacilityReportAudit",
                columns: new[] { "TenantId", "FacilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EXT_EnterpriseB2BContract_TenantId_EnterpriseId_ContractCode",
                schema: "shared",
                table: "EXT_EnterpriseB2BContract",
                columns: new[] { "TenantId", "EnterpriseId", "ContractCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EXT_EnterpriseB2BContract_TenantId_FacilityId",
                schema: "shared",
                table: "EXT_EnterpriseB2BContract",
                columns: new[] { "TenantId", "FacilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EXT_FacilityServicePriceList_TenantId_FacilityId_PriceListCode",
                schema: "shared",
                table: "EXT_FacilityServicePriceList",
                columns: new[] { "TenantId", "FacilityId", "PriceListCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EXT_FacilityServicePriceListLine_TenantId_FacilityId_PriceListId_ServiceItemCode",
                schema: "shared",
                table: "EXT_FacilityServicePriceListLine",
                columns: new[] { "TenantId", "FacilityId", "PriceListId", "ServiceItemCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EXT_LabCriticalValueEscalation_TenantId_FacilityId_LabResultId",
                schema: "shared",
                table: "EXT_LabCriticalValueEscalation",
                columns: new[] { "TenantId", "FacilityId", "LabResultId" });

            migrationBuilder.CreateIndex(
                name: "IX_EXT_ModuleIntegrationHandoff_TenantId_CorrelationId",
                schema: "shared",
                table: "EXT_ModuleIntegrationHandoff",
                columns: new[] { "TenantId", "CorrelationId" });

            migrationBuilder.CreateIndex(
                name: "IX_EXT_ModuleIntegrationHandoff_TenantId_FacilityId",
                schema: "shared",
                table: "EXT_ModuleIntegrationHandoff",
                columns: new[] { "TenantId", "FacilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EXT_TenantOnboardingStage_TenantId_FacilityId",
                schema: "shared",
                table: "EXT_TenantOnboardingStage",
                columns: new[] { "TenantId", "FacilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_EXT_TenantOnboardingStage_TenantId_StageCode",
                schema: "shared",
                table: "EXT_TenantOnboardingStage",
                columns: new[] { "TenantId", "StageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facility_TenantId_BusinessUnitId",
                schema: "shared",
                table: "Facility",
                columns: new[] { "TenantId", "BusinessUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_Facility_TenantId_FacilityCode",
                schema: "shared",
                table: "Facility",
                columns: new[] { "TenantId", "FacilityCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facility_TenantId_PrimaryAddressId",
                schema: "shared",
                table: "Facility",
                columns: new[] { "TenantId", "PrimaryAddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_Facility_TenantId_PrimaryContactId",
                schema: "shared",
                table: "Facility",
                columns: new[] { "TenantId", "PrimaryContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Admission_TenantId_FacilityId_AdmissionNo",
                schema: "hms",
                table: "HMS_Admission",
                columns: new[] { "TenantId", "FacilityId", "AdmissionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Admission_TenantId_FacilityId_BedId",
                schema: "hms",
                table: "HMS_Admission",
                columns: new[] { "TenantId", "FacilityId", "BedId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Admission_TenantId_PatientMasterId",
                schema: "hms",
                table: "HMS_Admission",
                columns: new[] { "TenantId", "PatientMasterId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_AdmissionTransfer_TenantId_FacilityId_AdmissionId",
                schema: "hms",
                table: "HMS_AdmissionTransfer",
                columns: new[] { "TenantId", "FacilityId", "AdmissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_AnesthesiaRecord_TenantId_FacilityId_SurgeryScheduleId",
                schema: "hms",
                table: "HMS_AnesthesiaRecord",
                columns: new[] { "TenantId", "FacilityId", "SurgeryScheduleId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Appointment_TenantId_FacilityId_AppointmentNo",
                schema: "hms",
                table: "HMS_Appointment",
                columns: new[] { "TenantId", "FacilityId", "AppointmentNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Appointment_TenantId_VisitTypeId",
                schema: "hms",
                table: "HMS_Appointment",
                columns: new[] { "TenantId", "VisitTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Bed_TenantId_FacilityId_WardId_BedCode",
                schema: "hms",
                table: "HMS_Bed",
                columns: new[] { "TenantId", "FacilityId", "WardId", "BedCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Claim_TenantId_FacilityId_BillingHeaderId",
                schema: "hms",
                table: "HMS_Claim",
                columns: new[] { "TenantId", "FacilityId", "BillingHeaderId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Claim_TenantId_FacilityId_ClaimNo",
                schema: "hms",
                table: "HMS_Claim",
                columns: new[] { "TenantId", "FacilityId", "ClaimNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Claim_TenantId_InsuranceProviderId",
                schema: "hms",
                table: "HMS_Claim",
                columns: new[] { "TenantId", "InsuranceProviderId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Claim_TenantId_PatientMasterId",
                schema: "hms",
                table: "HMS_Claim",
                columns: new[] { "TenantId", "PatientMasterId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_DoctorOrderAlert_TenantId_FacilityId_AdmissionId",
                schema: "hms",
                table: "HMS_DoctorOrderAlert",
                columns: new[] { "TenantId", "FacilityId", "AdmissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_DoctorOrderAlert_TenantId_FacilityId_VisitId",
                schema: "hms",
                table: "HMS_DoctorOrderAlert",
                columns: new[] { "TenantId", "FacilityId", "VisitId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_EmarEntry_TenantId_FacilityId_AdmissionId",
                schema: "hms",
                table: "HMS_EmarEntry",
                columns: new[] { "TenantId", "FacilityId", "AdmissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_HousekeepingStatus_TenantId_FacilityId_BedId",
                schema: "hms",
                table: "HMS_HousekeepingStatus",
                columns: new[] { "TenantId", "FacilityId", "BedId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_InsuranceProvider_TenantId_ProviderCode",
                schema: "hms",
                table: "HMS_InsuranceProvider",
                columns: new[] { "TenantId", "ProviderCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_OperationTheatre_TenantId_FacilityId_TheatreCode",
                schema: "hms",
                table: "HMS_OperationTheatre",
                columns: new[] { "TenantId", "FacilityId", "TheatreCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_OTConsumable_TenantId_FacilityId_SurgeryScheduleId",
                schema: "hms",
                table: "HMS_OTConsumable",
                columns: new[] { "TenantId", "FacilityId", "SurgeryScheduleId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PackageDefinition_TenantId_FacilityId_PackageCode",
                schema: "hms",
                table: "HMS_PackageDefinition",
                columns: new[] { "TenantId", "FacilityId", "PackageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PackageDefinitionLine_TenantId_FacilityId_PackageDefinitionId_LineNo",
                schema: "hms",
                table: "HMS_PackageDefinitionLine",
                columns: new[] { "TenantId", "FacilityId", "PackageDefinitionId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PatientFacilityLink_TenantId_PatientMasterId_FacilityId",
                schema: "hms",
                table: "HMS_PatientFacilityLink",
                columns: new[] { "TenantId", "PatientMasterId", "FacilityId" },
                unique: true,
                filter: "[FacilityId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PatientMaster_TenantId_UPID",
                schema: "hms",
                table: "HMS_PatientMaster",
                columns: new[] { "TenantId", "UPID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PostOpRecord_TenantId_FacilityId_SurgeryScheduleId",
                schema: "hms",
                table: "HMS_PostOpRecord",
                columns: new[] { "TenantId", "FacilityId", "SurgeryScheduleId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PreAuthorization_TenantId_FacilityId_PreAuthNo",
                schema: "hms",
                table: "HMS_PreAuthorization",
                columns: new[] { "TenantId", "FacilityId", "PreAuthNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PreAuthorization_TenantId_InsuranceProviderId",
                schema: "hms",
                table: "HMS_PreAuthorization",
                columns: new[] { "TenantId", "InsuranceProviderId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PreAuthorization_TenantId_PatientMasterId",
                schema: "hms",
                table: "HMS_PreAuthorization",
                columns: new[] { "TenantId", "PatientMasterId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_PricingRule_TenantId_FacilityId_RuleCode",
                schema: "hms",
                table: "HMS_PricingRule",
                columns: new[] { "TenantId", "FacilityId", "RuleCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_ProformaInvoice_TenantId_FacilityId_ProformaNo",
                schema: "hms",
                table: "HMS_ProformaInvoice",
                columns: new[] { "TenantId", "FacilityId", "ProformaNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_ProformaInvoice_TenantId_FacilityId_VisitId",
                schema: "hms",
                table: "HMS_ProformaInvoice",
                columns: new[] { "TenantId", "FacilityId", "VisitId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_ProformaInvoice_TenantId_PatientMasterId",
                schema: "hms",
                table: "HMS_ProformaInvoice",
                columns: new[] { "TenantId", "PatientMasterId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_SurgerySchedule_TenantId_FacilityId_OperationTheatreId",
                schema: "hms",
                table: "HMS_SurgerySchedule",
                columns: new[] { "TenantId", "FacilityId", "OperationTheatreId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_SurgerySchedule_TenantId_PatientMasterId",
                schema: "hms",
                table: "HMS_SurgerySchedule",
                columns: new[] { "TenantId", "PatientMasterId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Visit_TenantId_FacilityId_AppointmentId",
                schema: "hms",
                table: "HMS_Visit",
                columns: new[] { "TenantId", "FacilityId", "AppointmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Visit_TenantId_FacilityId_VisitNo",
                schema: "hms",
                table: "HMS_Visit",
                columns: new[] { "TenantId", "FacilityId", "VisitNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Visit_TenantId_VisitTypeId",
                schema: "hms",
                table: "HMS_Visit",
                columns: new[] { "TenantId", "VisitTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_HMS_VisitType_TenantId_VisitTypeCode",
                schema: "hms",
                table: "HMS_VisitType",
                columns: new[] { "TenantId", "VisitTypeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HMS_Ward_TenantId_FacilityId_WardCode",
                schema: "hms",
                table: "HMS_Ward",
                columns: new[] { "TenantId", "FacilityId", "WardCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identity_LoginAttempt_UserId",
                schema: "identity",
                table: "Identity_LoginAttempt",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_PasswordResetToken_UserId",
                schema: "identity",
                table: "Identity_PasswordResetToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_RefreshToken_TokenHash",
                schema: "identity",
                table: "Identity_RefreshToken",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_RefreshToken_UserId",
                schema: "identity",
                table: "Identity_RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_RolePermission_TenantId_PermissionId",
                schema: "identity",
                table: "Identity_RolePermission",
                columns: new[] { "TenantId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Identity_RolePermission_TenantId_RoleId",
                schema: "identity",
                table: "Identity_RolePermission",
                columns: new[] { "TenantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Identity_UserFacilityGrant_UserId_GrantFacilityId",
                schema: "identity",
                table: "Identity_UserFacilityGrant",
                columns: new[] { "UserId", "GrantFacilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identity_UserRole_TenantId_RoleId",
                schema: "identity",
                table: "Identity_UserRole",
                columns: new[] { "TenantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Identity_Users_TenantId_Email",
                schema: "identity",
                table: "Identity_Users",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LIS_AnalyzerResultHeader_TenantId_FacilityId_BarcodeValue",
                schema: "lis",
                table: "LIS_AnalyzerResultHeader",
                columns: new[] { "TenantId", "FacilityId", "BarcodeValue" });

            migrationBuilder.CreateIndex(
                name: "IX_LIS_AnalyzerResultLine_TenantId_FacilityId_AnalyzerResultHeaderId",
                schema: "lis",
                table: "LIS_AnalyzerResultLine",
                columns: new[] { "TenantId", "FacilityId", "AnalyzerResultHeaderId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogPackageParameterMap_TenantId_CatalogParameterId",
                schema: "lms",
                table: "LMS_CatalogPackageParameterMap",
                columns: new[] { "TenantId", "CatalogParameterId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogPackageParameterMap_TenantId_FacilityId_CatalogTestId",
                schema: "lms",
                table: "LMS_CatalogPackageParameterMap",
                columns: new[] { "TenantId", "FacilityId", "CatalogTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogPackageParameterMap_TenantId_FacilityId_TestPackageId",
                schema: "lms",
                table: "LMS_CatalogPackageParameterMap",
                columns: new[] { "TenantId", "FacilityId", "TestPackageId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogPackageTestLineMap_TenantId_FacilityId_CatalogTestId",
                schema: "lms",
                table: "LMS_CatalogPackageTestLineMap",
                columns: new[] { "TenantId", "FacilityId", "CatalogTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogPackageTestLineMap_TenantId_FacilityId_TestPackageId_LineNum",
                schema: "lms",
                table: "LMS_CatalogPackageTestLineMap",
                columns: new[] { "TenantId", "FacilityId", "TestPackageId", "LineNum" },
                unique: true,
                filter: "[FacilityId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogParameter_TenantId_FacilityId_ParameterCode",
                schema: "lms",
                table: "LMS_CatalogParameter",
                columns: new[] { "TenantId", "FacilityId", "ParameterCode" },
                unique: true,
                filter: "[FacilityId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogReferenceRange_TenantId_CatalogParameterId",
                schema: "lms",
                table: "LMS_CatalogReferenceRange",
                columns: new[] { "TenantId", "CatalogParameterId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogTest_TenantId_FacilityId_TestCode",
                schema: "lms",
                table: "LMS_CatalogTest",
                columns: new[] { "TenantId", "FacilityId", "TestCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogTestEquipmentMap_TenantId_FacilityId_CatalogTestId_EquipmentId",
                schema: "lms",
                table: "LMS_CatalogTestEquipmentMap",
                columns: new[] { "TenantId", "FacilityId", "CatalogTestId", "EquipmentId" },
                unique: true,
                filter: "[FacilityId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogTestEquipmentMap_TenantId_FacilityId_EquipmentId",
                schema: "lms",
                table: "LMS_CatalogTestEquipmentMap",
                columns: new[] { "TenantId", "FacilityId", "EquipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogTestParameterMap_TenantId_CatalogParameterId",
                schema: "lms",
                table: "LMS_CatalogTestParameterMap",
                columns: new[] { "TenantId", "CatalogParameterId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CatalogTestParameterMap_TenantId_FacilityId_CatalogTestId_CatalogParameterId",
                schema: "lms",
                table: "LMS_CatalogTestParameterMap",
                columns: new[] { "TenantId", "FacilityId", "CatalogTestId", "CatalogParameterId" },
                unique: true,
                filter: "[FacilityId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LMS_CollectionRequest_TenantId_FacilityId_RequestNo",
                schema: "lms",
                table: "LMS_CollectionRequest",
                columns: new[] { "TenantId", "FacilityId", "RequestNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LMS_EquipmentFacilityMapping_TenantId_EquipmentFacilityId_EquipmentId_MappedFacilityId",
                schema: "lms",
                table: "LMS_EquipmentFacilityMapping",
                columns: new[] { "TenantId", "EquipmentFacilityId", "EquipmentId", "MappedFacilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LMS_EquipmentTestMaster_TenantId_FacilityId_CatalogTestId",
                schema: "lms",
                table: "LMS_EquipmentTestMaster",
                columns: new[] { "TenantId", "FacilityId", "CatalogTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_EquipmentTestMaster_TenantId_FacilityId_EquipmentId_EquipmentAssayCode",
                schema: "lms",
                table: "LMS_EquipmentTestMaster",
                columns: new[] { "TenantId", "FacilityId", "EquipmentId", "EquipmentAssayCode" },
                unique: true,
                filter: "[FacilityId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LMS_EquipmentType_TenantId_TypeCode",
                schema: "lms",
                table: "LMS_EquipmentType",
                columns: new[] { "TenantId", "TypeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LMS_LabSampleBarcode_TenantId_BarcodeValue",
                schema: "lms",
                table: "LMS_LabSampleBarcode",
                columns: new[] { "TenantId", "BarcodeValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LMS_LabSampleBarcode_TenantId_FacilityId_TestBookingItemId",
                schema: "lms",
                table: "LMS_LabSampleBarcode",
                columns: new[] { "TenantId", "FacilityId", "TestBookingItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_LabTestBooking_TenantId_FacilityId_BookingNo",
                schema: "lms",
                table: "LMS_LabTestBooking",
                columns: new[] { "TenantId", "FacilityId", "BookingNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LMS_LabTestBookingItem_TenantId_FacilityId_CatalogTestId",
                schema: "lms",
                table: "LMS_LabTestBookingItem",
                columns: new[] { "TenantId", "FacilityId", "CatalogTestId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_LabTestBookingItem_TenantId_FacilityId_LabTestBookingId",
                schema: "lms",
                table: "LMS_LabTestBookingItem",
                columns: new[] { "TenantId", "FacilityId", "LabTestBookingId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_RiderTracking_TenantId_FacilityId_CollectionRequestId",
                schema: "lms",
                table: "LMS_RiderTracking",
                columns: new[] { "TenantId", "FacilityId", "CollectionRequestId" });

            migrationBuilder.CreateIndex(
                name: "IX_LMS_SampleTransport_TenantId_FacilityId_CollectionRequestId",
                schema: "lms",
                table: "LMS_SampleTransport",
                columns: new[] { "TenantId", "FacilityId", "CollectionRequestId" });

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_BatchStockLocation_TenantId_FacilityId_BatchStockId_InventoryLocationId",
                schema: "pharmacy",
                table: "Pharmacy_BatchStockLocation",
                columns: new[] { "TenantId", "FacilityId", "BatchStockId", "InventoryLocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_InventoryLocation_TenantId_FacilityId_LocationCode",
                schema: "pharmacy",
                table: "Pharmacy_InventoryLocation",
                columns: new[] { "TenantId", "FacilityId", "LocationCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_InventoryLocation_TenantId_FacilityId_ParentLocationId",
                schema: "pharmacy",
                table: "Pharmacy_InventoryLocation",
                columns: new[] { "TenantId", "FacilityId", "ParentLocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_ReorderPolicy_TenantId_FacilityId_BatchStockId",
                schema: "pharmacy",
                table: "Pharmacy_ReorderPolicy",
                columns: new[] { "TenantId", "FacilityId", "BatchStockId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_SalesReturn_TenantId_FacilityId_ReturnNo",
                schema: "pharmacy",
                table: "Pharmacy_SalesReturn",
                columns: new[] { "TenantId", "FacilityId", "ReturnNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacy_SalesReturnItem_TenantId_FacilityId_SalesReturnId",
                schema: "pharmacy",
                table: "Pharmacy_SalesReturnItem",
                columns: new[] { "TenantId", "FacilityId", "SalesReturnId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchStock",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "COM_NotificationLog",
                schema: "communication");

            migrationBuilder.DropTable(
                name: "COM_NotificationQueue",
                schema: "communication");

            migrationBuilder.DropTable(
                name: "COM_NotificationRecipient",
                schema: "communication");

            migrationBuilder.DropTable(
                name: "Composition",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "Department",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "EquipmentCalibration",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "EquipmentMaintenance",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "ExpiryTracking",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "EXT_CrossFacilityReportAudit",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "EXT_EnterpriseB2BContract",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "EXT_FacilityServicePriceListLine",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "EXT_LabCriticalValueEscalation",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "EXT_ModuleIntegrationHandoff",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "EXT_TenantOnboardingStage",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "GoodsReceipt",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "GoodsReceiptItems",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "HMS_AdmissionTransfer",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_AnesthesiaRecord",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_AppointmentQueue",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_AppointmentStatusHistory",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_BillingItems",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_Claim",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_ClinicalNotes",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_Diagnosis",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_DoctorOrderAlert",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_EmarEntry",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_HousekeepingStatus",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_OTConsumable",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PackageDefinitionLine",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PatientFacilityLink",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PaymentModes",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PaymentTransactions",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PostOpRecord",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PreAuthorization",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_Prescription",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PrescriptionItems",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PrescriptionNotes",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PricingRule",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_Procedure",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_ProformaInvoice",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_Vitals",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "IAM_PasswordResetToken",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "IAM_Permission",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "IAM_Role",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "IAM_RolePermission",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "IAM_UserAccount",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "IAM_UserFacilityScope",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "IAM_UserMfaFactor",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "IAM_UserRoleAssignment",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "IAM_UserSessionActivity",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "Identity_AccountLockoutState",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_LoginAttempt",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_PasswordResetToken",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_RefreshToken",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_RolePermission",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_UserFacilityGrant",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_UserProfile",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_UserRole",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "LabInventory",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LabInventoryTransactions",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LIS_AnalyzerResultLine",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_AnalyzerResultMap",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_LabOrder",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_LabOrderItems",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_LabResults",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_OrderStatusHistory",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_ReportDeliveryOtp",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_ReportDetails",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_ReportHeader",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_ReportLockState",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_ResultApproval",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_ResultHistory",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_SampleBarcode",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_SampleCollection",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_SampleLifecycleEvent",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_SampleTracking",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_SampleType",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_TestCategory",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_TestMaster",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_TestParameterProfile",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_TestParameters",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LIS_TestReferenceRanges",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LMS_AnalyticsDailyFacilityRollup",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_B2BCreditLedger",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_B2BPartner",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_B2BPartnerBillingStatement",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_B2BPartnerBillingStatementLine",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_B2BPartnerCreditProfile",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_B2BPartnerTestRate",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_CatalogPackageParameterMap",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_CatalogPackageTestLineMap",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_CatalogReferenceRange",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_CatalogTestEquipmentMap",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_CatalogTestParameterMap",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_EquipmentFacilityMapping",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_EquipmentTestMaster",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_EquipmentType",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_FinanceLedgerEntry",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_LabInvoiceHeader",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_LabInvoiceLine",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_LabOrderContext",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_LabPaymentTransaction",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_LabSampleBarcode",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_PatientWalletAccount",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_PatientWalletTransaction",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReagentBatch",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReagentConsumptionLog",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReagentMaster",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReferralDoctorProfile",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReferralFeeLedger",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReferralFeeRule",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReferralSettlement",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReferralSettlementLine",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReportDigitalSign",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReportPaymentGate",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ReportValidationStep",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_ResultDeltaCheck",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_RiderTracking",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_SampleTransport",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_TestPackageLine",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_TestPrice",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_TestReagentMap",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "Manufacturer",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "Medicine",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "MedicineBatch",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "MedicineCategory",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "MedicineComposition",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "Pharmacy_BatchStockLocation",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "Pharmacy_ControlledDrugRegister",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "Pharmacy_InventoryLocation",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "Pharmacy_ReorderPolicy",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "Pharmacy_SalesReturnItem",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "PharmacySales",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "PharmacySalesItems",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "PrescriptionMapping",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "ProcessingStages",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "PurchaseOrder",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "QCRecords",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "QCResults",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "SEC_DataChangeAuditLog",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "StockAdjustment",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "StockAdjustmentItems",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "StockLedger",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "StockTransfer",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "StockTransferItems",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "TechnicianAssignment",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "WorkQueue",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "COM_NotificationChannel",
                schema: "communication");

            migrationBuilder.DropTable(
                name: "EXT_FacilityServicePriceList",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "HMS_BillingHeader",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_Admission",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PackageDefinition",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_SurgerySchedule",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_InsuranceProvider",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_Visit",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "Identity_Permission",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_Role",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Identity_Users",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "LIS_AnalyzerResultHeader",
                schema: "lis");

            migrationBuilder.DropTable(
                name: "LMS_TestPackage",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_CatalogParameter",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "Equipment",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_LabTestBookingItem",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_CollectionRequest",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "Pharmacy_SalesReturn",
                schema: "pharmacy");

            migrationBuilder.DropTable(
                name: "COM_NotificationTemplate",
                schema: "communication");

            migrationBuilder.DropTable(
                name: "COM_Notification",
                schema: "communication");

            migrationBuilder.DropTable(
                name: "Facility",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "HMS_Bed",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_OperationTheatre",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_PatientMaster",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_Appointment",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "LMS_CatalogTest",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "LMS_LabTestBooking",
                schema: "lms");

            migrationBuilder.DropTable(
                name: "BusinessUnit",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "HMS_Ward",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "HMS_VisitType",
                schema: "hms");

            migrationBuilder.DropTable(
                name: "Company",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "Enterprise",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "Address",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "ContactDetails",
                schema: "shared");
        }
    }
}
