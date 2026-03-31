IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    IF SCHEMA_ID(N'shared') IS NULL EXEC(N'CREATE SCHEMA [shared];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    IF SCHEMA_ID(N'pharmacy') IS NULL EXEC(N'CREATE SCHEMA [pharmacy];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    IF SCHEMA_ID(N'communication') IS NULL EXEC(N'CREATE SCHEMA [communication];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    IF SCHEMA_ID(N'lms') IS NULL EXEC(N'CREATE SCHEMA [lms];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    IF SCHEMA_ID(N'hms') IS NULL EXEC(N'CREATE SCHEMA [hms];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    IF SCHEMA_ID(N'identity') IS NULL EXEC(N'CREATE SCHEMA [identity];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    IF SCHEMA_ID(N'lis') IS NULL EXEC(N'CREATE SCHEMA [lis];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[Address] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [AddressType] nvarchar(max) NULL,
        [Line1] nvarchar(250) NOT NULL,
        [Line2] nvarchar(max) NULL,
        [Area] nvarchar(max) NULL,
        [City] nvarchar(max) NULL,
        [StateProvince] nvarchar(max) NULL,
        [PostalCode] nvarchar(max) NULL,
        [CountryCode] nvarchar(max) NULL,
        [Latitude] decimal(18,2) NULL,
        [Longitude] decimal(18,2) NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Address] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Address_TenantId_Id] UNIQUE ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[BatchStock] (
        [Id] bigint NOT NULL IDENTITY,
        [MedicineBatchId] bigint NOT NULL,
        [CurrentQty] decimal(18,4) NOT NULL,
        [ReservedQty] decimal(18,4) NOT NULL,
        [AvailableQty] decimal(18,4) NOT NULL,
        [ReorderLevelQty] decimal(18,4) NULL,
        [LastUpdatedOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_BatchStock] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [communication].[COM_Notification] (
        [Id] bigint NOT NULL IDENTITY,
        [EventType] nvarchar(150) NOT NULL,
        [ReferenceId] bigint NULL,
        [ContextJson] nvarchar(max) NULL,
        [PriorityReferenceValueId] bigint NOT NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_COM_Notification] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_COM_Notification_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [communication].[COM_NotificationTemplate] (
        [Id] bigint NOT NULL IDENTITY,
        [TemplateCode] nvarchar(100) NOT NULL,
        [TemplateName] nvarchar(250) NOT NULL,
        [ChannelTypeReferenceValueId] bigint NOT NULL,
        [SubjectTemplate] nvarchar(max) NULL,
        [BodyTemplate] nvarchar(max) NOT NULL,
        [Version] int NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_COM_NotificationTemplate] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_COM_NotificationTemplate_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Composition] (
        [Id] bigint NOT NULL IDENTITY,
        [CompositionName] nvarchar(250) NOT NULL,
        [CompositionCode] nvarchar(120) NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Composition] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[ContactDetails] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [ContactType] nvarchar(50) NOT NULL,
        [ContactValue] nvarchar(300) NOT NULL,
        [CountryCode] nvarchar(max) NULL,
        [Extension] nvarchar(max) NULL,
        [IsPrimary] bit NOT NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_ContactDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_ContactDetails_TenantId_Id] UNIQUE ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[Equipment] (
        [Id] bigint NOT NULL IDENTITY,
        [EquipmentCode] nvarchar(80) NOT NULL,
        [EquipmentName] nvarchar(250) NOT NULL,
        [EquipmentTypeReferenceValueId] bigint NULL,
        [SerialNumber] nvarchar(120) NULL,
        [EquipmentNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Equipment] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Equipment_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[EquipmentCalibration] (
        [Id] bigint NOT NULL IDENTITY,
        [EquipmentId] bigint NOT NULL,
        [CalibratedOn] datetime2 NOT NULL,
        [CalibratorDoctorId] bigint NULL,
        [ResultNumeric] decimal(18,4) NULL,
        [ResultText] nvarchar(1000) NULL,
        [ValidUntil] datetime2 NULL,
        [IsWithinTolerance] bit NOT NULL,
        [Comments] nvarchar(max) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_EquipmentCalibration] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[EquipmentMaintenance] (
        [Id] bigint NOT NULL IDENTITY,
        [EquipmentId] bigint NOT NULL,
        [MaintenanceTypeReferenceValueId] bigint NULL,
        [MaintenanceStatusReferenceValueId] bigint NULL,
        [ScheduledOn] datetime2 NULL,
        [PerformedOn] datetime2 NULL,
        [PerformedByDoctorId] bigint NULL,
        [MaintenanceNotes] nvarchar(max) NULL,
        [NextDueOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_EquipmentMaintenance] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[ExpiryTracking] (
        [Id] bigint NOT NULL IDENTITY,
        [MedicineBatchId] bigint NOT NULL,
        [ExpiryDate] date NOT NULL,
        [DaysToExpiry] int NULL,
        [ExpiryAlertStatusReferenceValueId] bigint NULL,
        [LastReviewedOn] datetime2 NULL,
        [ReviewedByDoctorId] bigint NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_ExpiryTracking] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[GoodsReceipt] (
        [Id] bigint NOT NULL IDENTITY,
        [GoodsReceiptNo] nvarchar(60) NOT NULL,
        [PurchaseOrderId] bigint NOT NULL,
        [ReceivedOn] datetime2 NOT NULL,
        [ReceivedByDoctorId] bigint NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_GoodsReceipt] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[GoodsReceiptItems] (
        [Id] bigint NOT NULL IDENTITY,
        [GoodsReceiptId] bigint NOT NULL,
        [PurchaseOrderItemId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [MedicineId] bigint NOT NULL,
        [MedicineBatchId] bigint NOT NULL,
        [QuantityReceived] decimal(18,4) NOT NULL,
        [UnitId] bigint NULL,
        [PurchaseRate] decimal(18,4) NULL,
        [ExpiryDate] date NULL,
        [MRP] decimal(18,4) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_GoodsReceiptItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_AppointmentQueue] (
        [Id] bigint NOT NULL IDENTITY,
        [AppointmentId] bigint NOT NULL,
        [QueueToken] nvarchar(60) NOT NULL,
        [PositionInQueue] int NOT NULL,
        [QueueStatusReferenceValueId] bigint NOT NULL,
        [EnqueuedOn] datetime2 NOT NULL,
        [CheckedInOn] datetime2 NULL,
        [ExpectedServiceOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_AppointmentQueue] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_AppointmentStatusHistory] (
        [Id] bigint NOT NULL IDENTITY,
        [AppointmentId] bigint NOT NULL,
        [StatusValueId] bigint NOT NULL,
        [StatusOn] datetime2 NOT NULL,
        [StatusNote] nvarchar(1000) NULL,
        [ChangedByDoctorId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_AppointmentStatusHistory] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_BillingHeader] (
        [Id] bigint NOT NULL IDENTITY,
        [BillNo] nvarchar(60) NOT NULL,
        [VisitId] bigint NOT NULL,
        [PatientId] bigint NOT NULL,
        [BillDate] datetime2 NOT NULL,
        [BillingStatusReferenceValueId] bigint NOT NULL,
        [SubTotal] decimal(18,4) NULL,
        [TaxTotal] decimal(18,4) NULL,
        [DiscountTotal] decimal(18,4) NULL,
        [GrandTotal] decimal(18,4) NULL,
        [CurrencyCode] nvarchar(3) NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_BillingHeader] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_BillingHeader_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_BillingItems] (
        [Id] bigint NOT NULL IDENTITY,
        [BillingHeaderId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [ServiceTypeReferenceValueId] bigint NOT NULL,
        [Description] nvarchar(500) NULL,
        [Quantity] decimal(18,4) NOT NULL,
        [UnitPrice] decimal(18,4) NULL,
        [LineTotal] decimal(18,4) NULL,
        [LabOrderId] bigint NULL,
        [PrescriptionId] bigint NULL,
        [PharmacySalesId] bigint NULL,
        [ExternalReference] nvarchar(120) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_BillingItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_ClinicalNotes] (
        [Id] bigint NOT NULL IDENTITY,
        [VisitId] bigint NOT NULL,
        [NoteTypeReferenceValueId] bigint NOT NULL,
        [EncounterSection] nvarchar(150) NULL,
        [NoteText] nvarchar(max) NOT NULL,
        [StructuredPayload] nvarchar(max) NULL,
        [AuthorDoctorId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_ClinicalNotes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Diagnosis] (
        [Id] bigint NOT NULL IDENTITY,
        [VisitId] bigint NOT NULL,
        [DiagnosisTypeReferenceValueId] bigint NULL,
        [ICDSystem] nvarchar(30) NOT NULL,
        [ICDCode] nvarchar(30) NOT NULL,
        [ICDVersion] nvarchar(20) NULL,
        [ICDDescription] nvarchar(500) NULL,
        [DiagnosisOn] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Diagnosis] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_InsuranceProvider] (
        [Id] bigint NOT NULL IDENTITY,
        [ProviderCode] nvarchar(80) NOT NULL,
        [ProviderName] nvarchar(250) NOT NULL,
        [TpaCategoryReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_InsuranceProvider] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_InsuranceProvider_TenantId_Id] UNIQUE ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_OperationTheatre] (
        [Id] bigint NOT NULL IDENTITY,
        [TheatreCode] nvarchar(40) NOT NULL,
        [TheatreName] nvarchar(200) NOT NULL,
        [DepartmentId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_OperationTheatre] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_OperationTheatre_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PackageDefinition] (
        [Id] bigint NOT NULL IDENTITY,
        [PackageCode] nvarchar(80) NOT NULL,
        [PackageName] nvarchar(250) NOT NULL,
        [BundlePrice] decimal(18,4) NOT NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PackageDefinition] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_PackageDefinition_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PatientMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [UPID] nvarchar(40) NOT NULL,
        [SharedPatientId] bigint NULL,
        [FullName] nvarchar(250) NOT NULL,
        [DateOfBirth] datetime2 NULL,
        [GenderReferenceValueId] bigint NULL,
        [PrimaryPhone] nvarchar(40) NULL,
        [PrimaryEmail] nvarchar(200) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PatientMaster] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_PatientMaster_TenantId_Id] UNIQUE ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PaymentModes] (
        [Id] bigint NOT NULL IDENTITY,
        [ModeCode] nvarchar(40) NOT NULL,
        [ModeName] nvarchar(120) NOT NULL,
        [SortOrder] int NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PaymentModes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PaymentTransactions] (
        [Id] bigint NOT NULL IDENTITY,
        [BillingHeaderId] bigint NOT NULL,
        [PaymentModeId] bigint NOT NULL,
        [Amount] decimal(18,4) NOT NULL,
        [TransactionOn] datetime2 NOT NULL,
        [TransactionStatusReferenceValueId] bigint NOT NULL,
        [ReferenceNo] nvarchar(120) NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PaymentTransactions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Prescription] (
        [Id] bigint NOT NULL IDENTITY,
        [PrescriptionNo] nvarchar(60) NOT NULL,
        [VisitId] bigint NOT NULL,
        [PatientId] bigint NOT NULL,
        [DoctorId] bigint NOT NULL,
        [PrescribedOn] datetime2 NOT NULL,
        [PrescriptionStatusReferenceValueId] bigint NOT NULL,
        [ValidFrom] datetime2 NULL,
        [ValidTo] datetime2 NULL,
        [Indication] nvarchar(1000) NULL,
        [Notes] nvarchar(max) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Prescription] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PrescriptionItems] (
        [Id] bigint NOT NULL IDENTITY,
        [PrescriptionId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [MedicineId] bigint NOT NULL,
        [UnitId] bigint NULL,
        [Quantity] decimal(18,4) NULL,
        [DosageText] nvarchar(200) NULL,
        [FrequencyText] nvarchar(150) NULL,
        [DurationDays] int NULL,
        [RouteReferenceValueId] bigint NULL,
        [IsPRN] bit NOT NULL,
        [ItemNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PrescriptionItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PrescriptionNotes] (
        [Id] bigint NOT NULL IDENTITY,
        [PrescriptionId] bigint NOT NULL,
        [NoteTypeReferenceValueId] bigint NOT NULL,
        [NoteText] nvarchar(max) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PrescriptionNotes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PricingRule] (
        [Id] bigint NOT NULL IDENTITY,
        [RuleCode] nvarchar(80) NOT NULL,
        [RuleName] nvarchar(250) NOT NULL,
        [TariffTypeReferenceValueId] bigint NULL,
        [ServiceCode] nvarchar(80) NOT NULL,
        [UnitPrice] decimal(18,4) NOT NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PricingRule] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_PricingRule_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Procedure] (
        [Id] bigint NOT NULL IDENTITY,
        [VisitId] bigint NOT NULL,
        [ProcedureCode] nvarchar(50) NOT NULL,
        [ProcedureSystem] nvarchar(30) NULL,
        [ProcedureDescription] nvarchar(500) NULL,
        [PerformedOn] datetime2 NULL,
        [PerformedByDoctorId] bigint NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Procedure] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_VisitType] (
        [Id] bigint NOT NULL IDENTITY,
        [VisitTypeCode] nvarchar(80) NOT NULL,
        [VisitTypeName] nvarchar(250) NOT NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_VisitType] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_VisitType_TenantId_Id] UNIQUE ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Vitals] (
        [Id] bigint NOT NULL IDENTITY,
        [VisitId] bigint NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [VitalReferenceValueId] bigint NOT NULL,
        [ValueNumeric] decimal(18,4) NULL,
        [ValueNumeric2] decimal(18,4) NULL,
        [ValueText] nvarchar(200) NULL,
        [UnitId] bigint NULL,
        [RecordedByDoctorId] bigint NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Vitals] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Ward] (
        [Id] bigint NOT NULL IDENTITY,
        [WardCode] nvarchar(40) NOT NULL,
        [WardName] nvarchar(200) NOT NULL,
        [WardCategoryReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Ward] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_Ward_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_PasswordResetToken] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [TokenHash] nvarchar(256) NOT NULL,
        [ExpiresOn] datetime2 NOT NULL,
        [ConsumedOn] datetime2 NULL,
        [RequestChannelReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_IAM_PasswordResetToken] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_Permission] (
        [Id] bigint NOT NULL IDENTITY,
        [PermissionCode] nvarchar(120) NOT NULL,
        [PermissionName] nvarchar(250) NOT NULL,
        [ModuleCode] nvarchar(80) NULL,
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
        CONSTRAINT [PK_IAM_Permission] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_Role] (
        [Id] bigint NOT NULL IDENTITY,
        [RoleCode] nvarchar(80) NOT NULL,
        [RoleName] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsSystemRole] bit NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_IAM_Role] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_RolePermission] (
        [Id] bigint NOT NULL IDENTITY,
        [RoleId] bigint NOT NULL,
        [PermissionId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_IAM_RolePermission] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_UserAccount] (
        [Id] bigint NOT NULL IDENTITY,
        [LoginName] nvarchar(120) NOT NULL,
        [DisplayName] nvarchar(250) NULL,
        [Email] nvarchar(300) NULL,
        [Phone] nvarchar(50) NULL,
        [PasswordHash] nvarchar(500) NULL,
        [PatientId] bigint NULL,
        [DoctorId] bigint NULL,
        [UserStatusReferenceValueId] bigint NOT NULL,
        [LastLoginOn] datetime2 NULL,
        [RegistrationSourceReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_IAM_UserAccount] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_UserFacilityScope] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [GrantFacilityId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_IAM_UserFacilityScope] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_UserMfaFactor] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [MfaTypeReferenceValueId] bigint NOT NULL,
        [SecretPayload] nvarchar(max) NULL,
        [IsVerified] bit NOT NULL,
        [IsPrimary] bit NOT NULL,
        [LastUsedOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_IAM_UserMfaFactor] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_UserRoleAssignment] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [RoleId] bigint NOT NULL,
        [BusinessUnitId] bigint NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_IAM_UserRoleAssignment] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[IAM_UserSessionActivity] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [ActivityTypeReferenceValueId] bigint NOT NULL,
        [ActivityOn] datetime2 NOT NULL,
        [SessionTokenHash] nvarchar(256) NULL,
        [ClientIp] nvarchar(64) NULL,
        [UserAgent] nvarchar(500) NULL,
        [Success] bit NOT NULL,
        [FailureReason] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_IAM_UserSessionActivity] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_Permission] (
        [Id] bigint NOT NULL IDENTITY,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [PermissionCode] nvarchar(120) NOT NULL,
        [PermissionName] nvarchar(250) NOT NULL,
        [ModuleCode] nvarchar(80) NULL,
        [Description] nvarchar(500) NULL,
        CONSTRAINT [PK_Identity_Permission] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Identity_Permission_TenantId_Id] UNIQUE ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_Role] (
        [Id] bigint NOT NULL IDENTITY,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [RoleCode] nvarchar(80) NOT NULL,
        [RoleName] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsSystemRole] bit NOT NULL,
        CONSTRAINT [PK_Identity_Role] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Identity_Role_TenantId_Id] UNIQUE ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_Users] (
        [Id] bigint NOT NULL IDENTITY,
        [Email] nvarchar(320) NOT NULL,
        [PasswordHash] nvarchar(500) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [Role] nvarchar(128) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Identity_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LabInventory] (
        [Id] bigint NOT NULL IDENTITY,
        [InventoryItemCode] nvarchar(80) NOT NULL,
        [InventoryItemName] nvarchar(250) NOT NULL,
        [UnitId] bigint NULL,
        [CurrentQty] decimal(18,4) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LabInventory] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LabInventoryTransactions] (
        [Id] bigint NOT NULL IDENTITY,
        [LabInventoryId] bigint NOT NULL,
        [TransactionTypeReferenceValueId] bigint NULL,
        [QuantityDelta] decimal(18,4) NOT NULL,
        [TransactionOn] datetime2 NOT NULL,
        [PerformedByDoctorId] bigint NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LabInventoryTransactions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_AnalyzerResultHeader] (
        [Id] bigint NOT NULL IDENTITY,
        [BarcodeValue] nvarchar(120) NOT NULL,
        [LmsTestBookingItemId] bigint NOT NULL,
        [LmsCatalogTestId] bigint NOT NULL,
        [EquipmentId] bigint NULL,
        [EquipmentAssayCode] nvarchar(120) NULL,
        [ReceivedOn] datetime2 NOT NULL,
        [TechnicallyVerified] bit NOT NULL,
        [TechnicallyVerifiedOn] datetime2 NULL,
        [ReadyForDispatch] bit NOT NULL,
        [ResultStatusReferenceValueId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_AnalyzerResultHeader] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LIS_AnalyzerResultHeader_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_AnalyzerResultMap] (
        [Id] bigint NOT NULL IDENTITY,
        [EquipmentId] bigint NOT NULL,
        [ExternalTestCode] nvarchar(80) NOT NULL,
        [ExternalParameterCode] nvarchar(120) NOT NULL,
        [TestMasterId] bigint NOT NULL,
        [TestParameterId] bigint NULL,
        [ProtocolReferenceValueId] bigint NULL,
        [UnitOverrideId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_AnalyzerResultMap] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_LabOrder] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderNo] nvarchar(60) NOT NULL,
        [PatientId] bigint NOT NULL,
        [VisitId] bigint NULL,
        [OrderingDoctorId] bigint NULL,
        [DepartmentId] bigint NULL,
        [OrderedOn] datetime2 NOT NULL,
        [OrderStatusReferenceValueId] bigint NOT NULL,
        [PriorityReferenceValueId] bigint NULL,
        [ClinicalNotes] nvarchar(max) NULL,
        [RequestedCollectionOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_LabOrder] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_LabOrderItems] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderId] bigint NOT NULL,
        [TestMasterId] bigint NOT NULL,
        [SampleTypeId] bigint NULL,
        [LineNum] int NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [OrderItemStatusReferenceValueId] bigint NOT NULL,
        [SpecimenVolume] decimal(18,4) NULL,
        [SpecimenVolumeUnitId] bigint NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_LabOrderItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_LabResults] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderItemId] bigint NOT NULL,
        [TestParameterId] bigint NOT NULL,
        [ResultNumeric] decimal(18,4) NULL,
        [ResultText] nvarchar(max) NULL,
        [ResultUnitId] bigint NULL,
        [IsAbnormal] bit NOT NULL,
        [AbnormalFlagReferenceValueId] bigint NULL,
        [Notes] nvarchar(1000) NULL,
        [MeasuredOn] datetime2 NULL,
        [ResultStatusReferenceValueId] bigint NOT NULL,
        [EnteredByDoctorId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_LabResults] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_OrderStatusHistory] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderId] bigint NOT NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [StatusOn] datetime2 NOT NULL,
        [StatusNote] nvarchar(1000) NULL,
        [ChangedByDoctorId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_OrderStatusHistory] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_ReportDeliveryOtp] (
        [Id] bigint NOT NULL IDENTITY,
        [ReportHeaderId] bigint NOT NULL,
        [OtpHash] nvarchar(256) NOT NULL,
        [ExpiresOn] datetime2 NOT NULL,
        [ConsumedOn] datetime2 NULL,
        [DeliveryChannelReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_ReportDeliveryOtp] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_ReportDetails] (
        [Id] bigint NOT NULL IDENTITY,
        [ReportHeaderId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [TestMasterId] bigint NULL,
        [TestParameterId] bigint NULL,
        [ResultDisplayText] nvarchar(max) NULL,
        [ReferenceRangeDisplayText] nvarchar(max) NULL,
        [LineNotes] nvarchar(max) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_ReportDetails] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_ReportHeader] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderId] bigint NOT NULL,
        [ReportNo] nvarchar(60) NOT NULL,
        [ReportTypeReferenceValueId] bigint NULL,
        [ReportStatusReferenceValueId] bigint NOT NULL,
        [PreparedOn] datetime2 NULL,
        [ReviewedOn] datetime2 NULL,
        [IssuedOn] datetime2 NULL,
        [PreparedByDoctorId] bigint NULL,
        [ReviewedByDoctorId] bigint NULL,
        [IssuedByDoctorId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_ReportHeader] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_ReportLockState] (
        [Id] bigint NOT NULL IDENTITY,
        [ReportHeaderId] bigint NOT NULL,
        [IsLocked] bit NOT NULL,
        [LockedOn] datetime2 NULL,
        [LockedByUserId] bigint NULL,
        [LockReasonReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_ReportLockState] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_ResultApproval] (
        [Id] bigint NOT NULL IDENTITY,
        [LabResultsId] bigint NOT NULL,
        [ApprovalStatusReferenceValueId] bigint NOT NULL,
        [ApprovedByDoctorId] bigint NOT NULL,
        [ApprovedOn] datetime2 NOT NULL,
        [ApprovalNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_ResultApproval] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_ResultHistory] (
        [Id] bigint NOT NULL IDENTITY,
        [LabResultsId] bigint NOT NULL,
        [SnapshotResultNumeric] decimal(18,4) NULL,
        [SnapshotResultText] nvarchar(max) NULL,
        [SnapshotIsAbnormal] bit NOT NULL,
        [SnapshotAbnormalFlagReferenceValueId] bigint NULL,
        [SnapshotResultStatusReferenceValueId] bigint NOT NULL,
        [ChangedByDoctorId] bigint NULL,
        [ChangeReason] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_ResultHistory] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_SampleBarcode] (
        [Id] bigint NOT NULL IDENTITY,
        [SampleCollectionId] bigint NOT NULL,
        [BarcodeValue] nvarchar(120) NOT NULL,
        [QrPayload] nvarchar(max) NULL,
        [IdentifierTypeReferenceValueId] bigint NULL,
        [PrintedOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_SampleBarcode] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_SampleCollection] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderItemId] bigint NOT NULL,
        [SampleTypeId] bigint NOT NULL,
        [CollectedOn] datetime2 NOT NULL,
        [CollectedByDoctorId] bigint NULL,
        [CollectionDepartmentId] bigint NULL,
        [CollectedQuantity] decimal(18,4) NULL,
        [CollectedQuantityUnitId] bigint NULL,
        [CollectionNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_SampleCollection] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_SampleLifecycleEvent] (
        [Id] bigint NOT NULL IDENTITY,
        [SampleCollectionId] bigint NOT NULL,
        [LabOrderItemId] bigint NULL,
        [EventTypeReferenceValueId] bigint NOT NULL,
        [EventOn] datetime2 NOT NULL,
        [PlannedDueOn] datetime2 NULL,
        [TatBreached] bit NOT NULL,
        [LocationDepartmentId] bigint NULL,
        [EventNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_SampleLifecycleEvent] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_SampleTracking] (
        [Id] bigint NOT NULL IDENTITY,
        [SampleCollectionId] bigint NOT NULL,
        [TrackingNo] nvarchar(120) NOT NULL,
        [TrackingEventTypeReferenceValueId] bigint NULL,
        [TrackingStatusReferenceValueId] bigint NULL,
        [LocationDepartmentId] bigint NULL,
        [TrackedOn] datetime2 NOT NULL,
        [ScannedByDoctorId] bigint NULL,
        [TrackingNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_SampleTracking] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_SampleType] (
        [Id] bigint NOT NULL IDENTITY,
        [SampleTypeCode] nvarchar(80) NOT NULL,
        [SampleTypeName] nvarchar(250) NOT NULL,
        [Description] nvarchar(500) NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_SampleType] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_TestCategory] (
        [Id] bigint NOT NULL IDENTITY,
        [CategoryCode] nvarchar(80) NOT NULL,
        [CategoryName] nvarchar(250) NOT NULL,
        [ParentCategoryId] bigint NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_TestCategory] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_TestMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CategoryId] bigint NOT NULL,
        [SampleTypeId] bigint NULL,
        [TestCode] nvarchar(80) NOT NULL,
        [TestName] nvarchar(250) NOT NULL,
        [TestDescription] nvarchar(1000) NULL,
        [DefaultUnitId] bigint NULL,
        [IsQuantitative] bit NOT NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_TestMaster] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_TestParameterProfile] (
        [Id] bigint NOT NULL IDENTITY,
        [TestParameterId] bigint NOT NULL,
        [MethodReferenceValueId] bigint NULL,
        [CollectionMethodReferenceValueId] bigint NULL,
        [ContainerTypeReferenceValueId] bigint NULL,
        [AnalyzerChannelCode] nvarchar(80) NULL,
        [LoincCode] nvarchar(40) NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_TestParameterProfile] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_TestParameters] (
        [Id] bigint NOT NULL IDENTITY,
        [TestMasterId] bigint NOT NULL,
        [ParameterCode] nvarchar(100) NOT NULL,
        [ParameterName] nvarchar(300) NOT NULL,
        [DisplayOrder] int NOT NULL,
        [IsNumeric] bit NOT NULL,
        [UnitId] bigint NULL,
        [ParameterNotes] nvarchar(500) NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_TestParameters] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_TestReferenceRanges] (
        [Id] bigint NOT NULL IDENTITY,
        [TestParameterId] bigint NOT NULL,
        [SexReferenceValueId] bigint NULL,
        [AgeFromYears] int NULL,
        [AgeToYears] int NULL,
        [ReferenceRangeTypeReferenceValueId] bigint NULL,
        [MinValue] decimal(18,4) NULL,
        [MaxValue] decimal(18,4) NULL,
        [RangeUnitId] bigint NULL,
        [RangeNotes] nvarchar(800) NULL,
        [EffectiveFromDate] date NULL,
        [EffectiveToDate] date NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_TestReferenceRanges] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_AnalyticsDailyFacilityRollup] (
        [Id] bigint NOT NULL IDENTITY,
        [StatDate] datetime2 NOT NULL,
        [LabOrderCount] int NOT NULL,
        [ReportIssuedCount] int NOT NULL,
        [GrossRevenue] decimal(18,2) NOT NULL,
        [DiscountTotal] decimal(18,2) NOT NULL,
        [NetRevenue] decimal(18,2) NOT NULL,
        [ReferralFeeAccrued] decimal(18,2) NOT NULL,
        [AvgTatMinutes] int NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_AnalyticsDailyFacilityRollup] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_B2BCreditLedger] (
        [Id] bigint NOT NULL IDENTITY,
        [B2BPartnerCreditProfileId] bigint NOT NULL,
        [MovementTypeReferenceValueId] bigint NOT NULL,
        [AmountDelta] decimal(18,2) NOT NULL,
        [PostedOn] datetime2 NOT NULL,
        [LabInvoiceHeaderId] bigint NULL,
        [ReferenceNo] nvarchar(120) NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_B2BCreditLedger] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_B2BPartner] (
        [Id] bigint NOT NULL IDENTITY,
        [PartnerCode] nvarchar(80) NOT NULL,
        [PartnerName] nvarchar(250) NOT NULL,
        [PartnerCategoryReferenceValueId] bigint NULL,
        [PrimaryAddressId] bigint NULL,
        [PrimaryContactId] bigint NULL,
        [ContractReference] nvarchar(200) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_B2BPartner] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_B2BPartnerBillingStatement] (
        [Id] bigint NOT NULL IDENTITY,
        [StatementNo] nvarchar(60) NOT NULL,
        [B2BPartnerId] bigint NOT NULL,
        [PeriodStartOn] datetime2 NOT NULL,
        [PeriodEndOn] datetime2 NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [StatementStatusReferenceValueId] bigint NOT NULL,
        [IssuedOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_B2BPartnerBillingStatement] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_B2BPartnerBillingStatementLine] (
        [Id] bigint NOT NULL IDENTITY,
        [PartnerBillingStatementId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [LabInvoiceHeaderId] bigint NULL,
        [Description] nvarchar(500) NULL,
        [LineAmount] decimal(18,2) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_B2BPartnerBillingStatementLine] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_B2BPartnerCreditProfile] (
        [Id] bigint NOT NULL IDENTITY,
        [B2BPartnerId] bigint NOT NULL,
        [CreditLimitAmount] decimal(18,2) NOT NULL,
        [CreditCurrencyCode] nvarchar(3) NULL,
        [PaymentTermsDays] int NULL,
        [GracePeriodDays] int NULL,
        [UtilizedAmount] decimal(18,2) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_B2BPartnerCreditProfile] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_B2BPartnerTestRate] (
        [Id] bigint NOT NULL IDENTITY,
        [B2BPartnerId] bigint NOT NULL,
        [TestMasterId] bigint NOT NULL,
        [RateAmount] decimal(18,2) NULL,
        [DiscountPercent] decimal(18,2) NULL,
        [EffectiveFrom] datetime2 NOT NULL,
        [EffectiveTo] datetime2 NULL,
        [ContractDocumentRef] nvarchar(200) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_B2BPartnerTestRate] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_CatalogParameter] (
        [Id] bigint NOT NULL IDENTITY,
        [ParameterCode] nvarchar(100) NOT NULL,
        [ParameterName] nvarchar(300) NOT NULL,
        [IsNumeric] bit NOT NULL,
        [UnitId] bigint NULL,
        [ParameterNotes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_CatalogParameter] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_CatalogParameter_TenantId_Id] UNIQUE ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_CatalogTest] (
        [Id] bigint NOT NULL IDENTITY,
        [TestCode] nvarchar(80) NOT NULL,
        [TestName] nvarchar(250) NOT NULL,
        [TestDescription] nvarchar(1000) NULL,
        [DisciplineReferenceValueId] bigint NOT NULL,
        [SampleTypeReferenceValueId] bigint NULL,
        [IsRadiology] bit NOT NULL,
        [DefaultUnitId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_CatalogTest] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_CatalogTest_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_CollectionRequest] (
        [Id] bigint NOT NULL IDENTITY,
        [RequestNo] nvarchar(60) NOT NULL,
        [PatientId] bigint NOT NULL,
        [CollectionAddressJson] nvarchar(max) NULL,
        [RequestedWindowStart] datetime2 NULL,
        [RequestedWindowEnd] datetime2 NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [ColdChainRequired] bit NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_CollectionRequest] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_CollectionRequest_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_EquipmentType] (
        [Id] bigint NOT NULL IDENTITY,
        [TypeCode] nvarchar(80) NOT NULL,
        [TypeName] nvarchar(250) NOT NULL,
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
        CONSTRAINT [PK_LMS_EquipmentType] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_FinanceLedgerEntry] (
        [Id] bigint NOT NULL IDENTITY,
        [EntryDate] datetime2 NOT NULL,
        [AccountCategoryReferenceValueId] bigint NOT NULL,
        [SourceTypeReferenceValueId] bigint NOT NULL,
        [SourceId] bigint NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [DebitCreditReferenceValueId] bigint NULL,
        [PatientId] bigint NULL,
        [LabOrderId] bigint NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_FinanceLedgerEntry] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_LabInvoiceHeader] (
        [Id] bigint NOT NULL IDENTITY,
        [InvoiceNo] nvarchar(60) NOT NULL,
        [LabOrderId] bigint NOT NULL,
        [PatientId] bigint NOT NULL,
        [VisitId] bigint NULL,
        [InvoiceStatusReferenceValueId] bigint NOT NULL,
        [InvoiceDate] datetime2 NOT NULL,
        [SubTotal] decimal(18,2) NULL,
        [TaxTotal] decimal(18,2) NULL,
        [DiscountTotal] decimal(18,2) NULL,
        [GrandTotal] decimal(18,2) NULL,
        [AmountPaid] decimal(18,2) NOT NULL,
        [BalanceDue] decimal(18,2) NULL,
        [CurrencyCode] nvarchar(3) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_LabInvoiceHeader] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_LabInvoiceLine] (
        [Id] bigint NOT NULL IDENTITY,
        [LabInvoiceHeaderId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [LineTypeReferenceValueId] bigint NOT NULL,
        [LabOrderItemId] bigint NULL,
        [TestMasterId] bigint NULL,
        [TestPackageId] bigint NULL,
        [Description] nvarchar(500) NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NULL,
        [LineSubTotal] decimal(18,2) NULL,
        [TaxAmount] decimal(18,2) NULL,
        [LineTotal] decimal(18,2) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_LabInvoiceLine] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_LabOrderContext] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderId] bigint NOT NULL,
        [B2BPartnerId] bigint NULL,
        [ReferralDoctorProfileId] bigint NULL,
        [SampleSourceReferenceValueId] bigint NULL,
        [BookingChannelReferenceValueId] bigint NULL,
        [ExpectedReportOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_LabOrderContext] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_LabPaymentTransaction] (
        [Id] bigint NOT NULL IDENTITY,
        [LabInvoiceHeaderId] bigint NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [TransactionOn] datetime2 NOT NULL,
        [TransactionStatusReferenceValueId] bigint NOT NULL,
        [PaymentModeId] bigint NULL,
        [GatewayProviderReferenceValueId] bigint NULL,
        [ExternalTransactionId] nvarchar(200) NULL,
        [ReferenceNo] nvarchar(120) NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_LabPaymentTransaction] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_LabTestBooking] (
        [Id] bigint NOT NULL IDENTITY,
        [BookingNo] nvarchar(60) NOT NULL,
        [PatientId] bigint NOT NULL,
        [VisitId] bigint NULL,
        [SourceReferenceValueId] bigint NULL,
        [BookingNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_LabTestBooking] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_LabTestBooking_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_PatientWalletAccount] (
        [Id] bigint NOT NULL IDENTITY,
        [PatientId] bigint NOT NULL,
        [CurrencyCode] nvarchar(3) NULL,
        [CurrentBalance] decimal(18,2) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_PatientWalletAccount] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_PatientWalletTransaction] (
        [Id] bigint NOT NULL IDENTITY,
        [PatientWalletAccountId] bigint NOT NULL,
        [AmountDelta] decimal(18,2) NOT NULL,
        [WalletTxnTypeReferenceValueId] bigint NOT NULL,
        [TransactionOn] datetime2 NOT NULL,
        [LabInvoiceHeaderId] bigint NULL,
        [LabPaymentTransactionId] bigint NULL,
        [ReferenceNo] nvarchar(120) NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_PatientWalletTransaction] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReagentBatch] (
        [Id] bigint NOT NULL IDENTITY,
        [ReagentMasterId] bigint NOT NULL,
        [LotNo] nvarchar(120) NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [ReceivedOn] datetime2 NULL,
        [LabInventoryId] bigint NULL,
        [OpeningQuantity] decimal(18,2) NULL,
        [CurrentQuantity] decimal(18,2) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReagentBatch] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReagentConsumptionLog] (
        [Id] bigint NOT NULL IDENTITY,
        [ReagentBatchId] bigint NOT NULL,
        [QuantityConsumed] decimal(18,2) NOT NULL,
        [ConsumedOn] datetime2 NOT NULL,
        [LabOrderItemId] bigint NULL,
        [WorkQueueId] bigint NULL,
        [ConsumptionReasonReferenceValueId] bigint NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReagentConsumptionLog] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReagentMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [ReagentCode] nvarchar(80) NOT NULL,
        [ReagentName] nvarchar(250) NOT NULL,
        [DefaultUnitId] bigint NULL,
        [StorageNotes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReagentMaster] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReferralDoctorProfile] (
        [Id] bigint NOT NULL IDENTITY,
        [ReferralCode] nvarchar(80) NOT NULL,
        [DisplayName] nvarchar(250) NOT NULL,
        [LinkedDoctorId] bigint NULL,
        [HospitalAffiliation] nvarchar(300) NULL,
        [PrimaryContactId] bigint NULL,
        [PrimaryAddressId] bigint NULL,
        [ReferralTypeReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReferralDoctorProfile] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReferralFeeLedger] (
        [Id] bigint NOT NULL IDENTITY,
        [ReferralDoctorProfileId] bigint NOT NULL,
        [LabInvoiceHeaderId] bigint NOT NULL,
        [LabInvoiceLineId] bigint NULL,
        [LabOrderItemId] bigint NULL,
        [FeeAmount] decimal(18,2) NOT NULL,
        [LedgerStatusReferenceValueId] bigint NOT NULL,
        [AccruedOn] datetime2 NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReferralFeeLedger] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReferralFeeRule] (
        [Id] bigint NOT NULL IDENTITY,
        [ReferralDoctorProfileId] bigint NOT NULL,
        [FeeModeReferenceValueId] bigint NOT NULL,
        [FeeValue] decimal(18,2) NOT NULL,
        [ApplyScopeReferenceValueId] bigint NOT NULL,
        [TestMasterId] bigint NULL,
        [EffectiveFrom] datetime2 NOT NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReferralFeeRule] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReferralSettlement] (
        [Id] bigint NOT NULL IDENTITY,
        [SettlementNo] nvarchar(60) NOT NULL,
        [ReferralDoctorProfileId] bigint NOT NULL,
        [PeriodStartOn] datetime2 NOT NULL,
        [PeriodEndOn] datetime2 NOT NULL,
        [TotalSettledAmount] decimal(18,2) NOT NULL,
        [SettlementStatusReferenceValueId] bigint NOT NULL,
        [SettledOn] datetime2 NULL,
        [PaymentReferenceNo] nvarchar(120) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReferralSettlement] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReferralSettlementLine] (
        [Id] bigint NOT NULL IDENTITY,
        [ReferralSettlementId] bigint NOT NULL,
        [ReferralFeeLedgerId] bigint NOT NULL,
        [AppliedAmount] decimal(18,2) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReferralSettlementLine] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReportDigitalSign] (
        [Id] bigint NOT NULL IDENTITY,
        [ReportHeaderId] bigint NOT NULL,
        [SignerUserId] bigint NOT NULL,
        [SignedOn] datetime2 NOT NULL,
        [SignatureReference] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReportDigitalSign] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_ReportDigitalSign_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReportPaymentGate] (
        [Id] bigint NOT NULL IDENTITY,
        [ReportHeaderId] bigint NOT NULL,
        [LabInvoiceHeaderId] bigint NOT NULL,
        [MinimumPaidPercent] decimal(18,2) NOT NULL,
        [IsReleased] bit NOT NULL,
        [ReleasedOn] datetime2 NULL,
        [ReleaseReasonReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReportPaymentGate] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ReportValidationStep] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderId] bigint NOT NULL,
        [ValidationLevel] int NOT NULL,
        [ValidatorUserId] bigint NOT NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [ValidatedOn] datetime2 NULL,
        [Comments] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ReportValidationStep] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_ReportValidationStep_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_ResultDeltaCheck] (
        [Id] bigint NOT NULL IDENTITY,
        [CurrentLabResultId] bigint NOT NULL,
        [PriorLabResultId] bigint NOT NULL,
        [DeltaPercent] decimal(18,6) NULL,
        [Flagged] bit NOT NULL,
        [EvaluatedOn] datetime2 NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_ResultDeltaCheck] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_ResultDeltaCheck_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_TestPackage] (
        [Id] bigint NOT NULL IDENTITY,
        [PackageCode] nvarchar(80) NOT NULL,
        [PackageName] nvarchar(250) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_TestPackage] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_TestPackage_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_TestPackageLine] (
        [Id] bigint NOT NULL IDENTITY,
        [TestPackageId] bigint NOT NULL,
        [TestMasterId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [IsOptionalInPackage] bit NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_TestPackageLine] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_TestPrice] (
        [Id] bigint NOT NULL IDENTITY,
        [TestMasterId] bigint NULL,
        [TestPackageId] bigint NULL,
        [DepartmentId] bigint NULL,
        [PriceTierReferenceValueId] bigint NULL,
        [RateAmount] decimal(18,2) NOT NULL,
        [CurrencyCode] nvarchar(3) NULL,
        [EffectiveFrom] datetime2 NOT NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_TestPrice] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_TestReagentMap] (
        [Id] bigint NOT NULL IDENTITY,
        [TestMasterId] bigint NOT NULL,
        [ReagentMasterId] bigint NOT NULL,
        [QuantityPerTest] decimal(18,2) NOT NULL,
        [UnitId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_TestReagentMap] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Manufacturer] (
        [Id] bigint NOT NULL IDENTITY,
        [ManufacturerCode] nvarchar(80) NULL,
        [ManufacturerName] nvarchar(250) NOT NULL,
        [CountryCode] nvarchar(10) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Manufacturer] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Medicine] (
        [Id] bigint NOT NULL IDENTITY,
        [MedicineCode] nvarchar(80) NOT NULL,
        [MedicineName] nvarchar(300) NOT NULL,
        [CategoryId] bigint NOT NULL,
        [ManufacturerId] bigint NULL,
        [Strength] nvarchar(120) NULL,
        [DefaultUnitId] bigint NULL,
        [FormReferenceValueId] bigint NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Medicine] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[MedicineBatch] (
        [Id] bigint NOT NULL IDENTITY,
        [MedicineId] bigint NOT NULL,
        [BatchNo] nvarchar(120) NOT NULL,
        [ExpiryDate] date NULL,
        [MRP] decimal(18,4) NULL,
        [PurchaseRate] decimal(18,4) NULL,
        [ManufacturingDate] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_MedicineBatch] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[MedicineCategory] (
        [Id] bigint NOT NULL IDENTITY,
        [CategoryCode] nvarchar(80) NOT NULL,
        [CategoryName] nvarchar(250) NOT NULL,
        [Description] nvarchar(500) NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_MedicineCategory] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[MedicineComposition] (
        [Id] bigint NOT NULL IDENTITY,
        [MedicineId] bigint NOT NULL,
        [CompositionId] bigint NOT NULL,
        [Amount] decimal(18,4) NULL,
        [UnitId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_MedicineComposition] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Pharmacy_BatchStockLocation] (
        [Id] bigint NOT NULL IDENTITY,
        [BatchStockId] bigint NOT NULL,
        [InventoryLocationId] bigint NOT NULL,
        [QuantityOnHand] decimal(18,4) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Pharmacy_BatchStockLocation] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Pharmacy_BatchStockLocation_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Pharmacy_ControlledDrugRegister] (
        [Id] bigint NOT NULL IDENTITY,
        [PharmacySalesItemId] bigint NOT NULL,
        [PrescribingDoctorId] bigint NOT NULL,
        [PatientId] bigint NOT NULL,
        [PatientAcknowledged] bit NOT NULL,
        [PatientAcknowledgedOn] datetime2 NULL,
        [RegisterEntryOn] datetime2 NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Pharmacy_ControlledDrugRegister] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Pharmacy_ControlledDrugRegister_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Pharmacy_InventoryLocation] (
        [Id] bigint NOT NULL IDENTITY,
        [LocationCode] nvarchar(80) NOT NULL,
        [LocationName] nvarchar(250) NOT NULL,
        [LocationTypeReferenceValueId] bigint NOT NULL,
        [ParentLocationId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Pharmacy_InventoryLocation] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Pharmacy_InventoryLocation_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_Pharmacy_InventoryLocation_Pharmacy_InventoryLocation_TenantId_FacilityId_ParentLocationId] FOREIGN KEY ([TenantId], [FacilityId], [ParentLocationId]) REFERENCES [pharmacy].[Pharmacy_InventoryLocation] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Pharmacy_ReorderPolicy] (
        [Id] bigint NOT NULL IDENTITY,
        [BatchStockId] bigint NOT NULL,
        [LeadTimeDays] int NOT NULL,
        [EconomicOrderQty] decimal(18,4) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Pharmacy_ReorderPolicy] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Pharmacy_ReorderPolicy_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Pharmacy_SalesReturn] (
        [Id] bigint NOT NULL IDENTITY,
        [ReturnNo] nvarchar(60) NOT NULL,
        [OriginalSalesId] bigint NOT NULL,
        [ReturnReasonReferenceValueId] bigint NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [ReturnedOn] datetime2 NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Pharmacy_SalesReturn] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Pharmacy_SalesReturn_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[PharmacySales] (
        [Id] bigint NOT NULL IDENTITY,
        [SalesNo] nvarchar(60) NOT NULL,
        [PatientId] bigint NOT NULL,
        [VisitId] bigint NULL,
        [DoctorId] bigint NULL,
        [SalesDate] datetime2 NOT NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [CurrencyCode] nvarchar(3) NULL,
        [PaymentTotal] decimal(18,4) NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_PharmacySales] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[PharmacySalesItems] (
        [Id] bigint NOT NULL IDENTITY,
        [PharmacySalesId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [MedicineId] bigint NOT NULL,
        [MedicineBatchId] bigint NOT NULL,
        [QuantitySold] decimal(18,4) NOT NULL,
        [UnitId] bigint NULL,
        [UnitPrice] decimal(18,4) NULL,
        [LineTotal] decimal(18,4) NULL,
        [DispensedOn] datetime2 NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_PharmacySalesItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[PrescriptionMapping] (
        [Id] bigint NOT NULL IDENTITY,
        [PrescriptionId] bigint NOT NULL,
        [PharmacySalesId] bigint NOT NULL,
        [PrescriptionItemId] bigint NULL,
        [PharmacySalesItemId] bigint NULL,
        [MappedQty] decimal(18,4) NULL,
        [MappingNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_PrescriptionMapping] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[ProcessingStages] (
        [Id] bigint NOT NULL IDENTITY,
        [StageCode] nvarchar(80) NOT NULL,
        [StageName] nvarchar(250) NOT NULL,
        [SequenceNo] int NOT NULL,
        [StageNotes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_ProcessingStages] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[PurchaseOrder] (
        [Id] bigint NOT NULL IDENTITY,
        [PurchaseOrderNo] nvarchar(60) NOT NULL,
        [SupplierName] nvarchar(250) NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        [ExpectedOn] datetime2 NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[PurchaseOrderItems] (
        [Id] bigint NOT NULL IDENTITY,
        [PurchaseOrderId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [MedicineId] bigint NOT NULL,
        [QuantityOrdered] decimal(18,4) NOT NULL,
        [UnitId] bigint NULL,
        [PurchaseRate] decimal(18,4) NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_PurchaseOrderItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[QCRecords] (
        [Id] bigint NOT NULL IDENTITY,
        [QCTypeReferenceValueId] bigint NULL,
        [QCStatusReferenceValueId] bigint NOT NULL,
        [ScheduledOn] datetime2 NULL,
        [PerformedOn] datetime2 NULL,
        [PerformedByDoctorId] bigint NULL,
        [LotNo] nvarchar(120) NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_QCRecords] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[QCResults] (
        [Id] bigint NOT NULL IDENTITY,
        [QCRecordId] bigint NOT NULL,
        [TestParameterId] bigint NULL,
        [ResultNumeric] decimal(18,4) NULL,
        [ResultText] nvarchar(max) NULL,
        [ResultUnitId] bigint NULL,
        [IsPass] bit NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_QCResults] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[SEC_DataChangeAuditLog] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NULL,
        [ActionTypeReferenceValueId] bigint NOT NULL,
        [EntitySchema] nvarchar(50) NOT NULL,
        [EntityName] nvarchar(120) NOT NULL,
        [EntityKeyJson] nvarchar(500) NULL,
        [ChangeSummary] nvarchar(2000) NULL,
        [CorrelationId] nvarchar(80) NULL,
        [ClientIp] nvarchar(64) NULL,
        [UserAgent] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_SEC_DataChangeAuditLog] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[StockAdjustment] (
        [Id] bigint NOT NULL IDENTITY,
        [AdjustmentNo] nvarchar(60) NOT NULL,
        [AdjustmentOn] datetime2 NOT NULL,
        [AdjustmentTypeReferenceValueId] bigint NULL,
        [PerformedByDoctorId] bigint NULL,
        [ReasonNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_StockAdjustment] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[StockAdjustmentItems] (
        [Id] bigint NOT NULL IDENTITY,
        [StockAdjustmentId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [MedicineBatchId] bigint NOT NULL,
        [QuantityDelta] decimal(18,4) NOT NULL,
        [UnitCost] decimal(18,4) NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_StockAdjustmentItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[StockLedger] (
        [Id] bigint NOT NULL IDENTITY,
        [MedicineBatchId] bigint NOT NULL,
        [LedgerTypeReferenceValueId] bigint NULL,
        [TransactionOn] datetime2 NOT NULL,
        [QuantityDelta] decimal(18,4) NOT NULL,
        [BeforeQty] decimal(18,4) NOT NULL,
        [AfterQty] decimal(18,4) NOT NULL,
        [UnitCost] decimal(18,4) NULL,
        [TotalCost] decimal(18,4) NULL,
        [SourceReference] nvarchar(200) NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_StockLedger] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[StockTransfer] (
        [Id] bigint NOT NULL IDENTITY,
        [TransferNo] nvarchar(60) NOT NULL,
        [FromFacilityId] bigint NOT NULL,
        [ToFacilityId] bigint NOT NULL,
        [TransferOn] datetime2 NOT NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_StockTransfer] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[StockTransferItems] (
        [Id] bigint NOT NULL IDENTITY,
        [StockTransferId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [MedicineBatchId] bigint NOT NULL,
        [Quantity] decimal(18,4) NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_StockTransferItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[TechnicianAssignment] (
        [Id] bigint NOT NULL IDENTITY,
        [WorkQueueId] bigint NOT NULL,
        [TechnicianDoctorId] bigint NOT NULL,
        [AssignmentStatusReferenceValueId] bigint NOT NULL,
        [AssignedOn] datetime2 NOT NULL,
        [ReleasedOn] datetime2 NULL,
        [Notes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_TechnicianAssignment] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[WorkQueue] (
        [Id] bigint NOT NULL IDENTITY,
        [LabOrderItemId] bigint NOT NULL,
        [StageId] bigint NOT NULL,
        [PriorityReferenceValueId] bigint NULL,
        [QueueStatusReferenceValueId] bigint NOT NULL,
        [QueuedOn] datetime2 NOT NULL,
        [ClaimedOn] datetime2 NULL,
        [CompletedOn] datetime2 NULL,
        [AssignedByDoctorId] bigint NULL,
        [AssignedTechnicianDoctorId] bigint NULL,
        [QueueNotes] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_WorkQueue] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [communication].[COM_NotificationQueue] (
        [Id] bigint NOT NULL IDENTITY,
        [NotificationId] bigint NOT NULL,
        [ScheduledOn] datetime2 NOT NULL,
        [ProcessedOn] datetime2 NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_COM_NotificationQueue] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_COM_NotificationQueue_COM_Notification_TenantId_FacilityId_NotificationId] FOREIGN KEY ([TenantId], [FacilityId], [NotificationId]) REFERENCES [communication].[COM_Notification] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [communication].[COM_NotificationRecipient] (
        [Id] bigint NOT NULL IDENTITY,
        [NotificationId] bigint NOT NULL,
        [RecipientTypeReferenceValueId] bigint NOT NULL,
        [RecipientId] bigint NULL,
        [Email] nvarchar(300) NULL,
        [PhoneNumber] nvarchar(50) NULL,
        [IsPrimary] bit NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_COM_NotificationRecipient] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_COM_NotificationRecipient_COM_Notification_TenantId_FacilityId_NotificationId] FOREIGN KEY ([TenantId], [FacilityId], [NotificationId]) REFERENCES [communication].[COM_Notification] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [communication].[COM_NotificationChannel] (
        [Id] bigint NOT NULL IDENTITY,
        [NotificationId] bigint NOT NULL,
        [ChannelTypeReferenceValueId] bigint NOT NULL,
        [TemplateId] bigint NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [AttemptCount] int NOT NULL,
        [LastAttemptOn] datetime2 NULL,
        [SentOn] datetime2 NULL,
        [ErrorMessage] nvarchar(1000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_COM_NotificationChannel] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_COM_NotificationChannel_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_COM_NotificationChannel_COM_NotificationTemplate_TenantId_FacilityId_TemplateId] FOREIGN KEY ([TenantId], [FacilityId], [TemplateId]) REFERENCES [communication].[COM_NotificationTemplate] ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_COM_NotificationChannel_COM_Notification_TenantId_FacilityId_NotificationId] FOREIGN KEY ([TenantId], [FacilityId], [NotificationId]) REFERENCES [communication].[COM_Notification] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[Enterprise] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [EnterpriseCode] nvarchar(80) NOT NULL,
        [EnterpriseName] nvarchar(250) NOT NULL,
        [RegistrationDetails] nvarchar(max) NULL,
        [GlobalSettingsJson] nvarchar(max) NULL,
        [PrimaryAddressId] bigint NULL,
        [PrimaryContactId] bigint NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Enterprise] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Enterprise_TenantId_Id] UNIQUE ([TenantId], [Id]),
        CONSTRAINT [FK_Enterprise_Address_TenantId_PrimaryAddressId] FOREIGN KEY ([TenantId], [PrimaryAddressId]) REFERENCES [shared].[Address] ([TenantId], [Id]),
        CONSTRAINT [FK_Enterprise_ContactDetails_TenantId_PrimaryContactId] FOREIGN KEY ([TenantId], [PrimaryContactId]) REFERENCES [shared].[ContactDetails] ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_EquipmentFacilityMapping] (
        [Id] bigint NOT NULL IDENTITY,
        [EquipmentFacilityId] bigint NOT NULL,
        [EquipmentId] bigint NOT NULL,
        [MappedFacilityId] bigint NOT NULL,
        [MappingNotes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_EquipmentFacilityMapping] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LMS_EquipmentFacilityMapping_Equipment_TenantId_EquipmentFacilityId_EquipmentId] FOREIGN KEY ([TenantId], [EquipmentFacilityId], [EquipmentId]) REFERENCES [lms].[Equipment] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PackageDefinitionLine] (
        [Id] bigint NOT NULL IDENTITY,
        [PackageDefinitionId] bigint NOT NULL,
        [LineNo] int NOT NULL,
        [ServiceCode] nvarchar(80) NOT NULL,
        [Quantity] decimal(18,4) NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PackageDefinitionLine] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_PackageDefinitionLine_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_PackageDefinitionLine_HMS_PackageDefinition_TenantId_FacilityId_PackageDefinitionId] FOREIGN KEY ([TenantId], [FacilityId], [PackageDefinitionId]) REFERENCES [hms].[HMS_PackageDefinition] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Claim] (
        [Id] bigint NOT NULL IDENTITY,
        [ClaimNo] nvarchar(60) NOT NULL,
        [InsuranceProviderId] bigint NOT NULL,
        [PatientMasterId] bigint NOT NULL,
        [BillingHeaderId] bigint NULL,
        [SubmittedOn] datetime2 NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [ClaimAmount] decimal(18,4) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Claim] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_Claim_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_Claim_HMS_BillingHeader_TenantId_FacilityId_BillingHeaderId] FOREIGN KEY ([TenantId], [FacilityId], [BillingHeaderId]) REFERENCES [hms].[HMS_BillingHeader] ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_Claim_HMS_InsuranceProvider_TenantId_InsuranceProviderId] FOREIGN KEY ([TenantId], [InsuranceProviderId]) REFERENCES [hms].[HMS_InsuranceProvider] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_HMS_Claim_HMS_PatientMaster_TenantId_PatientMasterId] FOREIGN KEY ([TenantId], [PatientMasterId]) REFERENCES [hms].[HMS_PatientMaster] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PatientFacilityLink] (
        [Id] bigint NOT NULL IDENTITY,
        [PatientMasterId] bigint NOT NULL,
        [LinkedOn] datetime2 NOT NULL,
        [Notes] nvarchar(max) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PatientFacilityLink] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HMS_PatientFacilityLink_HMS_PatientMaster_TenantId_PatientMasterId] FOREIGN KEY ([TenantId], [PatientMasterId]) REFERENCES [hms].[HMS_PatientMaster] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PreAuthorization] (
        [Id] bigint NOT NULL IDENTITY,
        [PreAuthNo] nvarchar(60) NOT NULL,
        [InsuranceProviderId] bigint NOT NULL,
        [PatientMasterId] bigint NOT NULL,
        [RequestedOn] datetime2 NOT NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [ApprovedAmount] decimal(18,4) NULL,
        [Notes] nvarchar(2000) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PreAuthorization] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_PreAuthorization_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_PreAuthorization_HMS_InsuranceProvider_TenantId_InsuranceProviderId] FOREIGN KEY ([TenantId], [InsuranceProviderId]) REFERENCES [hms].[HMS_InsuranceProvider] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_HMS_PreAuthorization_HMS_PatientMaster_TenantId_PatientMasterId] FOREIGN KEY ([TenantId], [PatientMasterId]) REFERENCES [hms].[HMS_PatientMaster] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_SurgerySchedule] (
        [Id] bigint NOT NULL IDENTITY,
        [OperationTheatreId] bigint NOT NULL,
        [PatientMasterId] bigint NOT NULL,
        [SurgeonDoctorId] bigint NOT NULL,
        [ScheduledStartOn] datetime2 NOT NULL,
        [ScheduledEndOn] datetime2 NULL,
        [ProcedureSummary] nvarchar(500) NULL,
        [ScheduleStatusReferenceValueId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_SurgerySchedule] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_SurgerySchedule_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_SurgerySchedule_HMS_OperationTheatre_TenantId_FacilityId_OperationTheatreId] FOREIGN KEY ([TenantId], [FacilityId], [OperationTheatreId]) REFERENCES [hms].[HMS_OperationTheatre] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_HMS_SurgerySchedule_HMS_PatientMaster_TenantId_PatientMasterId] FOREIGN KEY ([TenantId], [PatientMasterId]) REFERENCES [hms].[HMS_PatientMaster] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Appointment] (
        [Id] bigint NOT NULL IDENTITY,
        [AppointmentNo] nvarchar(60) NOT NULL,
        [PatientId] bigint NOT NULL,
        [DoctorId] bigint NOT NULL,
        [DepartmentId] bigint NOT NULL,
        [VisitTypeId] bigint NULL,
        [AppointmentStatusValueId] bigint NOT NULL,
        [ScheduledStartOn] datetime2 NOT NULL,
        [ScheduledEndOn] datetime2 NULL,
        [PriorityReferenceValueId] bigint NULL,
        [Reason] nvarchar(1000) NULL,
        [EffectiveFrom] date NULL,
        [EffectiveTo] date NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Appointment] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_Appointment_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_Appointment_HMS_VisitType_TenantId_VisitTypeId] FOREIGN KEY ([TenantId], [VisitTypeId]) REFERENCES [hms].[HMS_VisitType] ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Bed] (
        [Id] bigint NOT NULL IDENTITY,
        [WardId] bigint NOT NULL,
        [BedCode] nvarchar(40) NOT NULL,
        [BedCategoryReferenceValueId] bigint NULL,
        [BedOperationalStatusReferenceValueId] bigint NOT NULL,
        [CurrentAdmissionId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Bed] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_Bed_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_Bed_HMS_Ward_TenantId_FacilityId_WardId] FOREIGN KEY ([TenantId], [FacilityId], [WardId]) REFERENCES [hms].[HMS_Ward] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_RolePermission] (
        [RoleId] bigint NOT NULL,
        [PermissionId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Identity_RolePermission] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_Identity_RolePermission_Identity_Permission_TenantId_PermissionId] FOREIGN KEY ([TenantId], [PermissionId]) REFERENCES [identity].[Identity_Permission] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Identity_RolePermission_Identity_Role_TenantId_RoleId] FOREIGN KEY ([TenantId], [RoleId]) REFERENCES [identity].[Identity_Role] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_AccountLockoutState] (
        [UserId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [FailedAttemptCount] int NOT NULL,
        [LockoutEndOn] datetime2 NULL,
        [LastFailedAttemptOn] datetime2 NULL,
        CONSTRAINT [PK_Identity_AccountLockoutState] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_Identity_AccountLockoutState_Identity_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[Identity_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_LoginAttempt] (
        [Id] bigint NOT NULL IDENTITY,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [UserId] bigint NULL,
        [EmailAttempted] nvarchar(320) NOT NULL,
        [Success] bit NOT NULL,
        [FailureReasonReferenceValueId] bigint NULL,
        [IpAddress] nvarchar(64) NULL,
        [UserAgent] nvarchar(512) NULL,
        [CorrelationId] uniqueidentifier NULL,
        CONSTRAINT [PK_Identity_LoginAttempt] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Identity_LoginAttempt_Identity_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[Identity_Users] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_PasswordResetToken] (
        [Id] bigint NOT NULL IDENTITY,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [UserId] bigint NOT NULL,
        [TokenHash] nvarchar(256) NOT NULL,
        [ExpiresOn] datetime2 NOT NULL,
        [ConsumedOn] datetime2 NULL,
        [RequestChannelReferenceValueId] bigint NULL,
        CONSTRAINT [PK_Identity_PasswordResetToken] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Identity_PasswordResetToken_Identity_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[Identity_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_RefreshToken] (
        [Id] bigint NOT NULL IDENTITY,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [UserId] bigint NOT NULL,
        [TokenFamilyId] uniqueidentifier NOT NULL,
        [TokenHash] nvarchar(256) NOT NULL,
        [ExpiresOn] datetime2 NOT NULL,
        [RevokedOn] datetime2 NULL,
        [ReplacedByTokenId] bigint NULL,
        [ClientIp] nvarchar(64) NULL,
        [UserAgent] nvarchar(512) NULL,
        CONSTRAINT [PK_Identity_RefreshToken] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Identity_RefreshToken_Identity_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[Identity_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_UserFacilityGrant] (
        [Id] bigint NOT NULL IDENTITY,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [UserId] bigint NOT NULL,
        [GrantFacilityId] bigint NOT NULL,
        CONSTRAINT [PK_Identity_UserFacilityGrant] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Identity_UserFacilityGrant_Identity_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[Identity_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_UserProfile] (
        [UserId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        [DisplayName] nvarchar(250) NULL,
        [Phone] nvarchar(50) NULL,
        [PreferredLocaleReferenceValueId] bigint NULL,
        [TimeZoneId] nvarchar(100) NULL,
        [AvatarUrl] nvarchar(500) NULL,
        [MfaEnabled] bit NOT NULL,
        CONSTRAINT [PK_Identity_UserProfile] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_Identity_UserProfile_Identity_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[Identity_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [identity].[Identity_UserRole] (
        [UserId] bigint NOT NULL,
        [RoleId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Identity_UserRole] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_Identity_UserRole_Identity_Role_TenantId_RoleId] FOREIGN KEY ([TenantId], [RoleId]) REFERENCES [identity].[Identity_Role] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Identity_UserRole_Identity_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [identity].[Identity_Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lis].[LIS_AnalyzerResultLine] (
        [Id] bigint NOT NULL IDENTITY,
        [AnalyzerResultHeaderId] bigint NOT NULL,
        [LmsCatalogParameterId] bigint NULL,
        [EquipmentResultCode] nvarchar(120) NULL,
        [ResultNumeric] decimal(18,2) NULL,
        [ResultText] nvarchar(2000) NULL,
        [ResultUnitId] bigint NULL,
        [LineStatusReferenceValueId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LIS_AnalyzerResultLine] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LIS_AnalyzerResultLine_LIS_AnalyzerResultHeader_TenantId_FacilityId_AnalyzerResultHeaderId] FOREIGN KEY ([TenantId], [FacilityId], [AnalyzerResultHeaderId]) REFERENCES [lis].[LIS_AnalyzerResultHeader] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_CatalogReferenceRange] (
        [Id] bigint NOT NULL IDENTITY,
        [CatalogParameterId] bigint NOT NULL,
        [SexReferenceValueId] bigint NULL,
        [AgeFromYears] int NULL,
        [AgeToYears] int NULL,
        [MinValue] decimal(18,2) NULL,
        [MaxValue] decimal(18,2) NULL,
        [RangeText] nvarchar(500) NULL,
        [RangeNotes] nvarchar(800) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_CatalogReferenceRange] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LMS_CatalogReferenceRange_LMS_CatalogParameter_TenantId_CatalogParameterId] FOREIGN KEY ([TenantId], [CatalogParameterId]) REFERENCES [lms].[LMS_CatalogParameter] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_CatalogTestEquipmentMap] (
        [Id] bigint NOT NULL IDENTITY,
        [CatalogTestId] bigint NOT NULL,
        [EquipmentId] bigint NOT NULL,
        [IsPreferred] bit NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_CatalogTestEquipmentMap] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LMS_CatalogTestEquipmentMap_Equipment_TenantId_FacilityId_EquipmentId] FOREIGN KEY ([TenantId], [FacilityId], [EquipmentId]) REFERENCES [lms].[Equipment] ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_LMS_CatalogTestEquipmentMap_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId] FOREIGN KEY ([TenantId], [FacilityId], [CatalogTestId]) REFERENCES [lms].[LMS_CatalogTest] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_CatalogTestParameterMap] (
        [Id] bigint NOT NULL IDENTITY,
        [CatalogTestId] bigint NOT NULL,
        [CatalogParameterId] bigint NOT NULL,
        [DisplayOrder] int NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_CatalogTestParameterMap] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LMS_CatalogTestParameterMap_LMS_CatalogParameter_TenantId_CatalogParameterId] FOREIGN KEY ([TenantId], [CatalogParameterId]) REFERENCES [lms].[LMS_CatalogParameter] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_LMS_CatalogTestParameterMap_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId] FOREIGN KEY ([TenantId], [FacilityId], [CatalogTestId]) REFERENCES [lms].[LMS_CatalogTest] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_EquipmentTestMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [EquipmentId] bigint NOT NULL,
        [CatalogTestId] bigint NOT NULL,
        [EquipmentAssayCode] nvarchar(120) NOT NULL,
        [EquipmentAssayName] nvarchar(300) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_EquipmentTestMaster] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LMS_EquipmentTestMaster_Equipment_TenantId_FacilityId_EquipmentId] FOREIGN KEY ([TenantId], [FacilityId], [EquipmentId]) REFERENCES [lms].[Equipment] ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_LMS_EquipmentTestMaster_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId] FOREIGN KEY ([TenantId], [FacilityId], [CatalogTestId]) REFERENCES [lms].[LMS_CatalogTest] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_RiderTracking] (
        [Id] bigint NOT NULL IDENTITY,
        [CollectionRequestId] bigint NOT NULL,
        [Latitude] decimal(9,6) NOT NULL,
        [Longitude] decimal(9,6) NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_RiderTracking] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_RiderTracking_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_LMS_RiderTracking_LMS_CollectionRequest_TenantId_FacilityId_CollectionRequestId] FOREIGN KEY ([TenantId], [FacilityId], [CollectionRequestId]) REFERENCES [lms].[LMS_CollectionRequest] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_SampleTransport] (
        [Id] bigint NOT NULL IDENTITY,
        [CollectionRequestId] bigint NOT NULL,
        [TemperatureCelsius] decimal(5,2) NULL,
        [RecordedOn] datetime2 NOT NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_SampleTransport] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_SampleTransport_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_LMS_SampleTransport_LMS_CollectionRequest_TenantId_FacilityId_CollectionRequestId] FOREIGN KEY ([TenantId], [FacilityId], [CollectionRequestId]) REFERENCES [lms].[LMS_CollectionRequest] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_LabTestBookingItem] (
        [Id] bigint NOT NULL IDENTITY,
        [LabTestBookingId] bigint NOT NULL,
        [CatalogTestId] bigint NOT NULL,
        [WorkflowStatusReferenceValueId] bigint NOT NULL,
        [LineNotes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_LabTestBookingItem] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_LMS_LabTestBookingItem_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_LMS_LabTestBookingItem_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId] FOREIGN KEY ([TenantId], [FacilityId], [CatalogTestId]) REFERENCES [lms].[LMS_CatalogTest] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_LMS_LabTestBookingItem_LMS_LabTestBooking_TenantId_FacilityId_LabTestBookingId] FOREIGN KEY ([TenantId], [FacilityId], [LabTestBookingId]) REFERENCES [lms].[LMS_LabTestBooking] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_CatalogPackageParameterMap] (
        [Id] bigint NOT NULL IDENTITY,
        [TestPackageId] bigint NOT NULL,
        [CatalogParameterId] bigint NOT NULL,
        [CatalogTestId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_CatalogPackageParameterMap] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LMS_CatalogPackageParameterMap_LMS_CatalogParameter_TenantId_CatalogParameterId] FOREIGN KEY ([TenantId], [CatalogParameterId]) REFERENCES [lms].[LMS_CatalogParameter] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_LMS_CatalogPackageParameterMap_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId] FOREIGN KEY ([TenantId], [FacilityId], [CatalogTestId]) REFERENCES [lms].[LMS_CatalogTest] ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_LMS_CatalogPackageParameterMap_LMS_TestPackage_TenantId_FacilityId_TestPackageId] FOREIGN KEY ([TenantId], [FacilityId], [TestPackageId]) REFERENCES [lms].[LMS_TestPackage] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_CatalogPackageTestLineMap] (
        [Id] bigint NOT NULL IDENTITY,
        [TestPackageId] bigint NOT NULL,
        [LineNum] int NOT NULL,
        [CatalogTestId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_CatalogPackageTestLineMap] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LMS_CatalogPackageTestLineMap_LMS_CatalogTest_TenantId_FacilityId_CatalogTestId] FOREIGN KEY ([TenantId], [FacilityId], [CatalogTestId]) REFERENCES [lms].[LMS_CatalogTest] ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_LMS_CatalogPackageTestLineMap_LMS_TestPackage_TenantId_FacilityId_TestPackageId] FOREIGN KEY ([TenantId], [FacilityId], [TestPackageId]) REFERENCES [lms].[LMS_TestPackage] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [pharmacy].[Pharmacy_SalesReturnItem] (
        [Id] bigint NOT NULL IDENTITY,
        [SalesReturnId] bigint NOT NULL,
        [OriginalSalesItemId] bigint NOT NULL,
        [QuantityReturned] decimal(18,4) NOT NULL,
        [ReconciliationStatusReferenceValueId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Pharmacy_SalesReturnItem] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Pharmacy_SalesReturnItem_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_Pharmacy_SalesReturnItem_Pharmacy_SalesReturn_TenantId_FacilityId_SalesReturnId] FOREIGN KEY ([TenantId], [FacilityId], [SalesReturnId]) REFERENCES [pharmacy].[Pharmacy_SalesReturn] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [communication].[COM_NotificationLog] (
        [Id] bigint NOT NULL IDENTITY,
        [NotificationChannelId] bigint NOT NULL,
        [AttemptNo] int NOT NULL,
        [RequestPayload] nvarchar(max) NULL,
        [ResponsePayload] nvarchar(max) NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_COM_NotificationLog] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_COM_NotificationLog_COM_NotificationChannel_TenantId_FacilityId_NotificationChannelId] FOREIGN KEY ([TenantId], [FacilityId], [NotificationChannelId]) REFERENCES [communication].[COM_NotificationChannel] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[Company] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [EnterpriseId] bigint NOT NULL,
        [CompanyCode] nvarchar(80) NOT NULL,
        [CompanyName] nvarchar(250) NOT NULL,
        [PAN] nvarchar(max) NULL,
        [GSTIN] nvarchar(max) NULL,
        [LegalIdentifier1] nvarchar(max) NULL,
        [LegalIdentifier2] nvarchar(max) NULL,
        [PrimaryAddressId] bigint NULL,
        [PrimaryContactId] bigint NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Company] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Company_TenantId_Id] UNIQUE ([TenantId], [Id]),
        CONSTRAINT [FK_Company_Address_TenantId_PrimaryAddressId] FOREIGN KEY ([TenantId], [PrimaryAddressId]) REFERENCES [shared].[Address] ([TenantId], [Id]),
        CONSTRAINT [FK_Company_ContactDetails_TenantId_PrimaryContactId] FOREIGN KEY ([TenantId], [PrimaryContactId]) REFERENCES [shared].[ContactDetails] ([TenantId], [Id]),
        CONSTRAINT [FK_Company_Enterprise_TenantId_EnterpriseId] FOREIGN KEY ([TenantId], [EnterpriseId]) REFERENCES [shared].[Enterprise] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_AnesthesiaRecord] (
        [Id] bigint NOT NULL IDENTITY,
        [SurgeryScheduleId] bigint NOT NULL,
        [AnesthesiologistDoctorId] bigint NULL,
        [RecordJson] nvarchar(max) NULL,
        [RecordedOn] datetime2 NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_AnesthesiaRecord] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_AnesthesiaRecord_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_AnesthesiaRecord_HMS_SurgerySchedule_TenantId_FacilityId_SurgeryScheduleId] FOREIGN KEY ([TenantId], [FacilityId], [SurgeryScheduleId]) REFERENCES [hms].[HMS_SurgerySchedule] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_OTConsumable] (
        [Id] bigint NOT NULL IDENTITY,
        [SurgeryScheduleId] bigint NOT NULL,
        [ItemCode] nvarchar(80) NOT NULL,
        [ItemName] nvarchar(250) NULL,
        [Quantity] decimal(18,4) NOT NULL,
        [UnitPrice] decimal(18,4) NULL,
        [Billable] bit NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_OTConsumable] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_OTConsumable_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_OTConsumable_HMS_SurgerySchedule_TenantId_FacilityId_SurgeryScheduleId] FOREIGN KEY ([TenantId], [FacilityId], [SurgeryScheduleId]) REFERENCES [hms].[HMS_SurgerySchedule] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_PostOpRecord] (
        [Id] bigint NOT NULL IDENTITY,
        [SurgeryScheduleId] bigint NOT NULL,
        [RecoveryNotes] nvarchar(max) NULL,
        [RecordedOn] datetime2 NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_PostOpRecord] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_PostOpRecord_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_PostOpRecord_HMS_SurgerySchedule_TenantId_FacilityId_SurgeryScheduleId] FOREIGN KEY ([TenantId], [FacilityId], [SurgeryScheduleId]) REFERENCES [hms].[HMS_SurgerySchedule] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Visit] (
        [Id] bigint NOT NULL IDENTITY,
        [VisitNo] nvarchar(60) NOT NULL,
        [AppointmentId] bigint NULL,
        [PatientId] bigint NOT NULL,
        [DoctorId] bigint NOT NULL,
        [DepartmentId] bigint NOT NULL,
        [VisitTypeId] bigint NOT NULL,
        [VisitStartOn] datetime2 NOT NULL,
        [VisitEndOn] datetime2 NULL,
        [ChiefComplaint] nvarchar(2000) NULL,
        [CurrentStatusReferenceValueId] bigint NOT NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Visit] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_Visit_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_Visit_HMS_Appointment_TenantId_FacilityId_AppointmentId] FOREIGN KEY ([TenantId], [FacilityId], [AppointmentId]) REFERENCES [hms].[HMS_Appointment] ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_Visit_HMS_VisitType_TenantId_VisitTypeId] FOREIGN KEY ([TenantId], [VisitTypeId]) REFERENCES [hms].[HMS_VisitType] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_Admission] (
        [Id] bigint NOT NULL IDENTITY,
        [AdmissionNo] nvarchar(60) NOT NULL,
        [PatientMasterId] bigint NOT NULL,
        [BedId] bigint NOT NULL,
        [AdmissionStatusReferenceValueId] bigint NOT NULL,
        [AdmittedOn] datetime2 NOT NULL,
        [DischargedOn] datetime2 NULL,
        [AttendingDoctorId] bigint NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_Admission] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_Admission_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_Admission_HMS_Bed_TenantId_FacilityId_BedId] FOREIGN KEY ([TenantId], [FacilityId], [BedId]) REFERENCES [hms].[HMS_Bed] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_HMS_Admission_HMS_PatientMaster_TenantId_PatientMasterId] FOREIGN KEY ([TenantId], [PatientMasterId]) REFERENCES [hms].[HMS_PatientMaster] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_HousekeepingStatus] (
        [Id] bigint NOT NULL IDENTITY,
        [BedId] bigint NOT NULL,
        [HousekeepingStatusReferenceValueId] bigint NOT NULL,
        [RecordedOn] datetime2 NOT NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_HousekeepingStatus] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HMS_HousekeepingStatus_HMS_Bed_TenantId_FacilityId_BedId] FOREIGN KEY ([TenantId], [FacilityId], [BedId]) REFERENCES [hms].[HMS_Bed] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [lms].[LMS_LabSampleBarcode] (
        [Id] bigint NOT NULL IDENTITY,
        [BarcodeValue] nvarchar(120) NOT NULL,
        [TestBookingItemId] bigint NOT NULL,
        [SampleTypeReferenceValueId] bigint NULL,
        [BarcodeStatusReferenceValueId] bigint NOT NULL,
        [RegisteredFromSystem] nvarchar(120) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_LMS_LabSampleBarcode] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LMS_LabSampleBarcode_LMS_LabTestBookingItem_TenantId_FacilityId_TestBookingItemId] FOREIGN KEY ([TenantId], [FacilityId], [TestBookingItemId]) REFERENCES [lms].[LMS_LabTestBookingItem] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[BusinessUnit] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [CompanyId] bigint NOT NULL,
        [BusinessUnitCode] nvarchar(80) NOT NULL,
        [BusinessUnitName] nvarchar(250) NOT NULL,
        [BusinessUnitType] nvarchar(50) NOT NULL,
        [RegionCode] nvarchar(max) NULL,
        [CountryCode] nvarchar(max) NULL,
        [PrimaryAddressId] bigint NULL,
        [PrimaryContactId] bigint NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_BusinessUnit] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_BusinessUnit_TenantId_Id] UNIQUE ([TenantId], [Id]),
        CONSTRAINT [FK_BusinessUnit_Address_TenantId_PrimaryAddressId] FOREIGN KEY ([TenantId], [PrimaryAddressId]) REFERENCES [shared].[Address] ([TenantId], [Id]),
        CONSTRAINT [FK_BusinessUnit_Company_TenantId_CompanyId] FOREIGN KEY ([TenantId], [CompanyId]) REFERENCES [shared].[Company] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BusinessUnit_ContactDetails_TenantId_PrimaryContactId] FOREIGN KEY ([TenantId], [PrimaryContactId]) REFERENCES [shared].[ContactDetails] ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_ProformaInvoice] (
        [Id] bigint NOT NULL IDENTITY,
        [ProformaNo] nvarchar(60) NOT NULL,
        [PatientMasterId] bigint NULL,
        [VisitId] bigint NULL,
        [GrandTotal] decimal(18,4) NOT NULL,
        [StatusReferenceValueId] bigint NOT NULL,
        [LinesJson] nvarchar(max) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_ProformaInvoice] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_HMS_ProformaInvoice_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_ProformaInvoice_HMS_PatientMaster_TenantId_PatientMasterId] FOREIGN KEY ([TenantId], [PatientMasterId]) REFERENCES [hms].[HMS_PatientMaster] ([TenantId], [Id]),
        CONSTRAINT [FK_HMS_ProformaInvoice_HMS_Visit_TenantId_FacilityId_VisitId] FOREIGN KEY ([TenantId], [FacilityId], [VisitId]) REFERENCES [hms].[HMS_Visit] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_AdmissionTransfer] (
        [Id] bigint NOT NULL IDENTITY,
        [AdmissionId] bigint NOT NULL,
        [FromBedId] bigint NOT NULL,
        [ToBedId] bigint NOT NULL,
        [TransferredOn] datetime2 NOT NULL,
        [Reason] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_AdmissionTransfer] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HMS_AdmissionTransfer_HMS_Admission_TenantId_FacilityId_AdmissionId] FOREIGN KEY ([TenantId], [FacilityId], [AdmissionId]) REFERENCES [hms].[HMS_Admission] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_DoctorOrderAlert] (
        [Id] bigint NOT NULL IDENTITY,
        [VisitId] bigint NULL,
        [AdmissionId] bigint NULL,
        [DoctorId] bigint NOT NULL,
        [AlertTypeReferenceValueId] bigint NOT NULL,
        [Message] nvarchar(1000) NOT NULL,
        [AcknowledgedOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_DoctorOrderAlert] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HMS_DoctorOrderAlert_HMS_Admission_TenantId_FacilityId_AdmissionId] FOREIGN KEY ([TenantId], [FacilityId], [AdmissionId]) REFERENCES [hms].[HMS_Admission] ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_HMS_DoctorOrderAlert_HMS_Visit_TenantId_FacilityId_VisitId] FOREIGN KEY ([TenantId], [FacilityId], [VisitId]) REFERENCES [hms].[HMS_Visit] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [hms].[HMS_EmarEntry] (
        [Id] bigint NOT NULL IDENTITY,
        [AdmissionId] bigint NOT NULL,
        [MedicationCode] nvarchar(80) NOT NULL,
        [ScheduledOn] datetime2 NOT NULL,
        [AdministeredOn] datetime2 NULL,
        [AdministrationStatusReferenceValueId] bigint NOT NULL,
        [NurseUserId] bigint NULL,
        [Notes] nvarchar(500) NULL,
        [TenantId] bigint NOT NULL,
        [FacilityId] bigint NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_HMS_EmarEntry] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HMS_EmarEntry_HMS_Admission_TenantId_FacilityId_AdmissionId] FOREIGN KEY ([TenantId], [FacilityId], [AdmissionId]) REFERENCES [hms].[HMS_Admission] ([TenantId], [FacilityId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[Facility] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [BusinessUnitId] bigint NOT NULL,
        [FacilityCode] nvarchar(80) NOT NULL,
        [FacilityName] nvarchar(250) NOT NULL,
        [FacilityType] nvarchar(50) NOT NULL,
        [LicenseDetails] nvarchar(max) NULL,
        [TimeZoneId] nvarchar(max) NULL,
        [GeoCode] nvarchar(max) NULL,
        [PrimaryAddressId] bigint NULL,
        [PrimaryContactId] bigint NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Facility] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Facility_TenantId_Id] UNIQUE ([TenantId], [Id]),
        CONSTRAINT [FK_Facility_Address_TenantId_PrimaryAddressId] FOREIGN KEY ([TenantId], [PrimaryAddressId]) REFERENCES [shared].[Address] ([TenantId], [Id]),
        CONSTRAINT [FK_Facility_BusinessUnit_TenantId_BusinessUnitId] FOREIGN KEY ([TenantId], [BusinessUnitId]) REFERENCES [shared].[BusinessUnit] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Facility_ContactDetails_TenantId_PrimaryContactId] FOREIGN KEY ([TenantId], [PrimaryContactId]) REFERENCES [shared].[ContactDetails] ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[Department] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityParentId] bigint NOT NULL,
        [FacilityId] bigint NOT NULL,
        [DepartmentCode] nvarchar(80) NOT NULL,
        [DepartmentName] nvarchar(250) NOT NULL,
        [DepartmentType] nvarchar(50) NOT NULL,
        [ParentDepartmentId] bigint NULL,
        [PrimaryAddressId] bigint NULL,
        [PrimaryContactId] bigint NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Department] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Department_TenantId_Id] UNIQUE ([TenantId], [Id]),
        CONSTRAINT [FK_Department_Address_TenantId_PrimaryAddressId] FOREIGN KEY ([TenantId], [PrimaryAddressId]) REFERENCES [shared].[Address] ([TenantId], [Id]),
        CONSTRAINT [FK_Department_ContactDetails_TenantId_PrimaryContactId] FOREIGN KEY ([TenantId], [PrimaryContactId]) REFERENCES [shared].[ContactDetails] ([TenantId], [Id]),
        CONSTRAINT [FK_Department_Department_TenantId_ParentDepartmentId] FOREIGN KEY ([TenantId], [ParentDepartmentId]) REFERENCES [shared].[Department] ([TenantId], [Id]),
        CONSTRAINT [FK_Department_Facility_TenantId_FacilityParentId] FOREIGN KEY ([TenantId], [FacilityParentId]) REFERENCES [shared].[Facility] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[EXT_CrossFacilityReportAudit] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [ReportCode] nvarchar(80) NOT NULL,
        [ReportName] nvarchar(250) NULL,
        [FacilityScopeJson] nvarchar(max) NULL,
        [FilterJson] nvarchar(max) NULL,
        [ResultRowCount] int NULL,
        [CompletedOn] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_EXT_CrossFacilityReportAudit] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EXT_CrossFacilityReportAudit_Facility_TenantId_FacilityId] FOREIGN KEY ([TenantId], [FacilityId]) REFERENCES [shared].[Facility] ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[EXT_EnterpriseB2BContract] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [EnterpriseId] bigint NOT NULL,
        [PartnerType] nvarchar(50) NOT NULL,
        [PartnerName] nvarchar(250) NOT NULL,
        [ContractCode] nvarchar(80) NOT NULL,
        [TermsJson] nvarchar(max) NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_EXT_EnterpriseB2BContract] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EXT_EnterpriseB2BContract_Enterprise_TenantId_EnterpriseId] FOREIGN KEY ([TenantId], [EnterpriseId]) REFERENCES [shared].[Enterprise] ([TenantId], [Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EXT_EnterpriseB2BContract_Facility_TenantId_FacilityId] FOREIGN KEY ([TenantId], [FacilityId]) REFERENCES [shared].[Facility] ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[EXT_FacilityServicePriceList] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NOT NULL,
        [PriceListCode] nvarchar(80) NOT NULL,
        [PriceListName] nvarchar(250) NOT NULL,
        [ServiceModule] nvarchar(30) NOT NULL,
        [PartnerReferenceCode] nvarchar(80) NULL,
        [CurrencyCode] nvarchar(10) NOT NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_EXT_FacilityServicePriceList] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_EXT_FacilityServicePriceList_TenantId_FacilityId_Id] UNIQUE ([TenantId], [FacilityId], [Id]),
        CONSTRAINT [FK_EXT_FacilityServicePriceList_Facility_TenantId_FacilityId] FOREIGN KEY ([TenantId], [FacilityId]) REFERENCES [shared].[Facility] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[EXT_LabCriticalValueEscalation] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NOT NULL,
        [LabOrderId] bigint NULL,
        [LabOrderItemId] bigint NULL,
        [LabResultId] bigint NULL,
        [EscalationLevel] int NOT NULL,
        [ChannelCode] nvarchar(40) NOT NULL,
        [RecipientSummary] nvarchar(500) NULL,
        [DispatchedOn] datetime2 NULL,
        [AcknowledgedOn] datetime2 NULL,
        [OutcomeCode] nvarchar(40) NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_EXT_LabCriticalValueEscalation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EXT_LabCriticalValueEscalation_Facility_TenantId_FacilityId] FOREIGN KEY ([TenantId], [FacilityId]) REFERENCES [shared].[Facility] ([TenantId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[EXT_ModuleIntegrationHandoff] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [CorrelationId] nvarchar(64) NOT NULL,
        [SourceModule] nvarchar(30) NOT NULL,
        [TargetModule] nvarchar(30) NOT NULL,
        [EntityType] nvarchar(80) NOT NULL,
        [SourceEntityId] bigint NULL,
        [TargetEntityId] bigint NULL,
        [StatusCode] nvarchar(40) NOT NULL,
        [DetailJson] nvarchar(max) NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_EXT_ModuleIntegrationHandoff] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EXT_ModuleIntegrationHandoff_Facility_TenantId_FacilityId] FOREIGN KEY ([TenantId], [FacilityId]) REFERENCES [shared].[Facility] ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[EXT_TenantOnboardingStage] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NULL,
        [StageCode] nvarchar(80) NOT NULL,
        [StageStatus] nvarchar(40) NOT NULL,
        [CompletedOn] datetime2 NULL,
        [MetadataJson] nvarchar(max) NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_EXT_TenantOnboardingStage] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EXT_TenantOnboardingStage_Facility_TenantId_FacilityId] FOREIGN KEY ([TenantId], [FacilityId]) REFERENCES [shared].[Facility] ([TenantId], [Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE TABLE [shared].[EXT_FacilityServicePriceListLine] (
        [Id] bigint NOT NULL IDENTITY,
        [FacilityId] bigint NOT NULL,
        [PriceListId] bigint NOT NULL,
        [ServiceItemCode] nvarchar(80) NOT NULL,
        [ServiceItemName] nvarchar(250) NULL,
        [UnitPrice] decimal(18,4) NOT NULL,
        [TaxCategoryCode] nvarchar(40) NULL,
        [TenantId] bigint NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_EXT_FacilityServicePriceListLine] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EXT_FacilityServicePriceListLine_EXT_FacilityServicePriceList_TenantId_FacilityId_PriceListId] FOREIGN KEY ([TenantId], [FacilityId], [PriceListId]) REFERENCES [shared].[EXT_FacilityServicePriceList] ([TenantId], [FacilityId], [Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BusinessUnit_TenantId_BusinessUnitCode] ON [shared].[BusinessUnit] ([TenantId], [BusinessUnitCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_BusinessUnit_TenantId_CompanyId] ON [shared].[BusinessUnit] ([TenantId], [CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_BusinessUnit_TenantId_PrimaryAddressId] ON [shared].[BusinessUnit] ([TenantId], [PrimaryAddressId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_BusinessUnit_TenantId_PrimaryContactId] ON [shared].[BusinessUnit] ([TenantId], [PrimaryContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_COM_NotificationChannel_TenantId_FacilityId_NotificationId] ON [communication].[COM_NotificationChannel] ([TenantId], [FacilityId], [NotificationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_COM_NotificationChannel_TenantId_FacilityId_TemplateId] ON [communication].[COM_NotificationChannel] ([TenantId], [FacilityId], [TemplateId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_COM_NotificationLog_TenantId_FacilityId_NotificationChannelId] ON [communication].[COM_NotificationLog] ([TenantId], [FacilityId], [NotificationChannelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_COM_NotificationQueue_TenantId_FacilityId_NotificationId] ON [communication].[COM_NotificationQueue] ([TenantId], [FacilityId], [NotificationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_COM_NotificationRecipient_TenantId_FacilityId_NotificationId] ON [communication].[COM_NotificationRecipient] ([TenantId], [FacilityId], [NotificationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Company_TenantId_CompanyCode] ON [shared].[Company] ([TenantId], [CompanyCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Company_TenantId_EnterpriseId] ON [shared].[Company] ([TenantId], [EnterpriseId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Company_TenantId_PrimaryAddressId] ON [shared].[Company] ([TenantId], [PrimaryAddressId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Company_TenantId_PrimaryContactId] ON [shared].[Company] ([TenantId], [PrimaryContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Department_TenantId_DepartmentCode] ON [shared].[Department] ([TenantId], [DepartmentCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Department_TenantId_FacilityParentId] ON [shared].[Department] ([TenantId], [FacilityParentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Department_TenantId_ParentDepartmentId] ON [shared].[Department] ([TenantId], [ParentDepartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Department_TenantId_PrimaryAddressId] ON [shared].[Department] ([TenantId], [PrimaryAddressId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Department_TenantId_PrimaryContactId] ON [shared].[Department] ([TenantId], [PrimaryContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Enterprise_TenantId_EnterpriseCode] ON [shared].[Enterprise] ([TenantId], [EnterpriseCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Enterprise_TenantId_PrimaryAddressId] ON [shared].[Enterprise] ([TenantId], [PrimaryAddressId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Enterprise_TenantId_PrimaryContactId] ON [shared].[Enterprise] ([TenantId], [PrimaryContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_EXT_CrossFacilityReportAudit_TenantId_FacilityId] ON [shared].[EXT_CrossFacilityReportAudit] ([TenantId], [FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EXT_EnterpriseB2BContract_TenantId_EnterpriseId_ContractCode] ON [shared].[EXT_EnterpriseB2BContract] ([TenantId], [EnterpriseId], [ContractCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_EXT_EnterpriseB2BContract_TenantId_FacilityId] ON [shared].[EXT_EnterpriseB2BContract] ([TenantId], [FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EXT_FacilityServicePriceList_TenantId_FacilityId_PriceListCode] ON [shared].[EXT_FacilityServicePriceList] ([TenantId], [FacilityId], [PriceListCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EXT_FacilityServicePriceListLine_TenantId_FacilityId_PriceListId_ServiceItemCode] ON [shared].[EXT_FacilityServicePriceListLine] ([TenantId], [FacilityId], [PriceListId], [ServiceItemCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_EXT_LabCriticalValueEscalation_TenantId_FacilityId_LabResultId] ON [shared].[EXT_LabCriticalValueEscalation] ([TenantId], [FacilityId], [LabResultId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_EXT_ModuleIntegrationHandoff_TenantId_CorrelationId] ON [shared].[EXT_ModuleIntegrationHandoff] ([TenantId], [CorrelationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_EXT_ModuleIntegrationHandoff_TenantId_FacilityId] ON [shared].[EXT_ModuleIntegrationHandoff] ([TenantId], [FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_EXT_TenantOnboardingStage_TenantId_FacilityId] ON [shared].[EXT_TenantOnboardingStage] ([TenantId], [FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EXT_TenantOnboardingStage_TenantId_StageCode] ON [shared].[EXT_TenantOnboardingStage] ([TenantId], [StageCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Facility_TenantId_BusinessUnitId] ON [shared].[Facility] ([TenantId], [BusinessUnitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Facility_TenantId_FacilityCode] ON [shared].[Facility] ([TenantId], [FacilityCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Facility_TenantId_PrimaryAddressId] ON [shared].[Facility] ([TenantId], [PrimaryAddressId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Facility_TenantId_PrimaryContactId] ON [shared].[Facility] ([TenantId], [PrimaryContactId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_Admission_TenantId_FacilityId_AdmissionNo] ON [hms].[HMS_Admission] ([TenantId], [FacilityId], [AdmissionNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_Admission_TenantId_FacilityId_BedId] ON [hms].[HMS_Admission] ([TenantId], [FacilityId], [BedId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_Admission_TenantId_PatientMasterId] ON [hms].[HMS_Admission] ([TenantId], [PatientMasterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_AdmissionTransfer_TenantId_FacilityId_AdmissionId] ON [hms].[HMS_AdmissionTransfer] ([TenantId], [FacilityId], [AdmissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_AnesthesiaRecord_TenantId_FacilityId_SurgeryScheduleId] ON [hms].[HMS_AnesthesiaRecord] ([TenantId], [FacilityId], [SurgeryScheduleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_Appointment_TenantId_FacilityId_AppointmentNo] ON [hms].[HMS_Appointment] ([TenantId], [FacilityId], [AppointmentNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_Appointment_TenantId_VisitTypeId] ON [hms].[HMS_Appointment] ([TenantId], [VisitTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_Bed_TenantId_FacilityId_WardId_BedCode] ON [hms].[HMS_Bed] ([TenantId], [FacilityId], [WardId], [BedCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_Claim_TenantId_FacilityId_BillingHeaderId] ON [hms].[HMS_Claim] ([TenantId], [FacilityId], [BillingHeaderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_Claim_TenantId_FacilityId_ClaimNo] ON [hms].[HMS_Claim] ([TenantId], [FacilityId], [ClaimNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_Claim_TenantId_InsuranceProviderId] ON [hms].[HMS_Claim] ([TenantId], [InsuranceProviderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_Claim_TenantId_PatientMasterId] ON [hms].[HMS_Claim] ([TenantId], [PatientMasterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_DoctorOrderAlert_TenantId_FacilityId_AdmissionId] ON [hms].[HMS_DoctorOrderAlert] ([TenantId], [FacilityId], [AdmissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_DoctorOrderAlert_TenantId_FacilityId_VisitId] ON [hms].[HMS_DoctorOrderAlert] ([TenantId], [FacilityId], [VisitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_EmarEntry_TenantId_FacilityId_AdmissionId] ON [hms].[HMS_EmarEntry] ([TenantId], [FacilityId], [AdmissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_HousekeepingStatus_TenantId_FacilityId_BedId] ON [hms].[HMS_HousekeepingStatus] ([TenantId], [FacilityId], [BedId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_InsuranceProvider_TenantId_ProviderCode] ON [hms].[HMS_InsuranceProvider] ([TenantId], [ProviderCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_OperationTheatre_TenantId_FacilityId_TheatreCode] ON [hms].[HMS_OperationTheatre] ([TenantId], [FacilityId], [TheatreCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_OTConsumable_TenantId_FacilityId_SurgeryScheduleId] ON [hms].[HMS_OTConsumable] ([TenantId], [FacilityId], [SurgeryScheduleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_PackageDefinition_TenantId_FacilityId_PackageCode] ON [hms].[HMS_PackageDefinition] ([TenantId], [FacilityId], [PackageCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_PackageDefinitionLine_TenantId_FacilityId_PackageDefinitionId_LineNo] ON [hms].[HMS_PackageDefinitionLine] ([TenantId], [FacilityId], [PackageDefinitionId], [LineNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_HMS_PatientFacilityLink_TenantId_PatientMasterId_FacilityId] ON [hms].[HMS_PatientFacilityLink] ([TenantId], [PatientMasterId], [FacilityId]) WHERE [FacilityId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_PatientMaster_TenantId_UPID] ON [hms].[HMS_PatientMaster] ([TenantId], [UPID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_PostOpRecord_TenantId_FacilityId_SurgeryScheduleId] ON [hms].[HMS_PostOpRecord] ([TenantId], [FacilityId], [SurgeryScheduleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_PreAuthorization_TenantId_FacilityId_PreAuthNo] ON [hms].[HMS_PreAuthorization] ([TenantId], [FacilityId], [PreAuthNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_PreAuthorization_TenantId_InsuranceProviderId] ON [hms].[HMS_PreAuthorization] ([TenantId], [InsuranceProviderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_PreAuthorization_TenantId_PatientMasterId] ON [hms].[HMS_PreAuthorization] ([TenantId], [PatientMasterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_PricingRule_TenantId_FacilityId_RuleCode] ON [hms].[HMS_PricingRule] ([TenantId], [FacilityId], [RuleCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_ProformaInvoice_TenantId_FacilityId_ProformaNo] ON [hms].[HMS_ProformaInvoice] ([TenantId], [FacilityId], [ProformaNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_ProformaInvoice_TenantId_FacilityId_VisitId] ON [hms].[HMS_ProformaInvoice] ([TenantId], [FacilityId], [VisitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_ProformaInvoice_TenantId_PatientMasterId] ON [hms].[HMS_ProformaInvoice] ([TenantId], [PatientMasterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_SurgerySchedule_TenantId_FacilityId_OperationTheatreId] ON [hms].[HMS_SurgerySchedule] ([TenantId], [FacilityId], [OperationTheatreId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_SurgerySchedule_TenantId_PatientMasterId] ON [hms].[HMS_SurgerySchedule] ([TenantId], [PatientMasterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_Visit_TenantId_FacilityId_AppointmentId] ON [hms].[HMS_Visit] ([TenantId], [FacilityId], [AppointmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_Visit_TenantId_FacilityId_VisitNo] ON [hms].[HMS_Visit] ([TenantId], [FacilityId], [VisitNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_HMS_Visit_TenantId_VisitTypeId] ON [hms].[HMS_Visit] ([TenantId], [VisitTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_VisitType_TenantId_VisitTypeCode] ON [hms].[HMS_VisitType] ([TenantId], [VisitTypeCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HMS_Ward_TenantId_FacilityId_WardCode] ON [hms].[HMS_Ward] ([TenantId], [FacilityId], [WardCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Identity_LoginAttempt_UserId] ON [identity].[Identity_LoginAttempt] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Identity_PasswordResetToken_UserId] ON [identity].[Identity_PasswordResetToken] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Identity_RefreshToken_TokenHash] ON [identity].[Identity_RefreshToken] ([TokenHash]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Identity_RefreshToken_UserId] ON [identity].[Identity_RefreshToken] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Identity_RolePermission_TenantId_PermissionId] ON [identity].[Identity_RolePermission] ([TenantId], [PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Identity_RolePermission_TenantId_RoleId] ON [identity].[Identity_RolePermission] ([TenantId], [RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Identity_UserFacilityGrant_UserId_GrantFacilityId] ON [identity].[Identity_UserFacilityGrant] ([UserId], [GrantFacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Identity_UserRole_TenantId_RoleId] ON [identity].[Identity_UserRole] ([TenantId], [RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Identity_Users_TenantId_Email] ON [identity].[Identity_Users] ([TenantId], [Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LIS_AnalyzerResultHeader_TenantId_FacilityId_BarcodeValue] ON [lis].[LIS_AnalyzerResultHeader] ([TenantId], [FacilityId], [BarcodeValue]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LIS_AnalyzerResultLine_TenantId_FacilityId_AnalyzerResultHeaderId] ON [lis].[LIS_AnalyzerResultLine] ([TenantId], [FacilityId], [AnalyzerResultHeaderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_CatalogPackageParameterMap_TenantId_CatalogParameterId] ON [lms].[LMS_CatalogPackageParameterMap] ([TenantId], [CatalogParameterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_CatalogPackageParameterMap_TenantId_FacilityId_CatalogTestId] ON [lms].[LMS_CatalogPackageParameterMap] ([TenantId], [FacilityId], [CatalogTestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_CatalogPackageParameterMap_TenantId_FacilityId_TestPackageId] ON [lms].[LMS_CatalogPackageParameterMap] ([TenantId], [FacilityId], [TestPackageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_CatalogPackageTestLineMap_TenantId_FacilityId_CatalogTestId] ON [lms].[LMS_CatalogPackageTestLineMap] ([TenantId], [FacilityId], [CatalogTestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_LMS_CatalogPackageTestLineMap_TenantId_FacilityId_TestPackageId_LineNum] ON [lms].[LMS_CatalogPackageTestLineMap] ([TenantId], [FacilityId], [TestPackageId], [LineNum]) WHERE [FacilityId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_LMS_CatalogParameter_TenantId_FacilityId_ParameterCode] ON [lms].[LMS_CatalogParameter] ([TenantId], [FacilityId], [ParameterCode]) WHERE [FacilityId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_CatalogReferenceRange_TenantId_CatalogParameterId] ON [lms].[LMS_CatalogReferenceRange] ([TenantId], [CatalogParameterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LMS_CatalogTest_TenantId_FacilityId_TestCode] ON [lms].[LMS_CatalogTest] ([TenantId], [FacilityId], [TestCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_LMS_CatalogTestEquipmentMap_TenantId_FacilityId_CatalogTestId_EquipmentId] ON [lms].[LMS_CatalogTestEquipmentMap] ([TenantId], [FacilityId], [CatalogTestId], [EquipmentId]) WHERE [FacilityId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_CatalogTestEquipmentMap_TenantId_FacilityId_EquipmentId] ON [lms].[LMS_CatalogTestEquipmentMap] ([TenantId], [FacilityId], [EquipmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_CatalogTestParameterMap_TenantId_CatalogParameterId] ON [lms].[LMS_CatalogTestParameterMap] ([TenantId], [CatalogParameterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_LMS_CatalogTestParameterMap_TenantId_FacilityId_CatalogTestId_CatalogParameterId] ON [lms].[LMS_CatalogTestParameterMap] ([TenantId], [FacilityId], [CatalogTestId], [CatalogParameterId]) WHERE [FacilityId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LMS_CollectionRequest_TenantId_FacilityId_RequestNo] ON [lms].[LMS_CollectionRequest] ([TenantId], [FacilityId], [RequestNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LMS_EquipmentFacilityMapping_TenantId_EquipmentFacilityId_EquipmentId_MappedFacilityId] ON [lms].[LMS_EquipmentFacilityMapping] ([TenantId], [EquipmentFacilityId], [EquipmentId], [MappedFacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_EquipmentTestMaster_TenantId_FacilityId_CatalogTestId] ON [lms].[LMS_EquipmentTestMaster] ([TenantId], [FacilityId], [CatalogTestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_LMS_EquipmentTestMaster_TenantId_FacilityId_EquipmentId_EquipmentAssayCode] ON [lms].[LMS_EquipmentTestMaster] ([TenantId], [FacilityId], [EquipmentId], [EquipmentAssayCode]) WHERE [FacilityId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LMS_EquipmentType_TenantId_TypeCode] ON [lms].[LMS_EquipmentType] ([TenantId], [TypeCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LMS_LabSampleBarcode_TenantId_BarcodeValue] ON [lms].[LMS_LabSampleBarcode] ([TenantId], [BarcodeValue]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_LabSampleBarcode_TenantId_FacilityId_TestBookingItemId] ON [lms].[LMS_LabSampleBarcode] ([TenantId], [FacilityId], [TestBookingItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LMS_LabTestBooking_TenantId_FacilityId_BookingNo] ON [lms].[LMS_LabTestBooking] ([TenantId], [FacilityId], [BookingNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_LabTestBookingItem_TenantId_FacilityId_CatalogTestId] ON [lms].[LMS_LabTestBookingItem] ([TenantId], [FacilityId], [CatalogTestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_LabTestBookingItem_TenantId_FacilityId_LabTestBookingId] ON [lms].[LMS_LabTestBookingItem] ([TenantId], [FacilityId], [LabTestBookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_RiderTracking_TenantId_FacilityId_CollectionRequestId] ON [lms].[LMS_RiderTracking] ([TenantId], [FacilityId], [CollectionRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_LMS_SampleTransport_TenantId_FacilityId_CollectionRequestId] ON [lms].[LMS_SampleTransport] ([TenantId], [FacilityId], [CollectionRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Pharmacy_BatchStockLocation_TenantId_FacilityId_BatchStockId_InventoryLocationId] ON [pharmacy].[Pharmacy_BatchStockLocation] ([TenantId], [FacilityId], [BatchStockId], [InventoryLocationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Pharmacy_InventoryLocation_TenantId_FacilityId_LocationCode] ON [pharmacy].[Pharmacy_InventoryLocation] ([TenantId], [FacilityId], [LocationCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_InventoryLocation_TenantId_FacilityId_ParentLocationId] ON [pharmacy].[Pharmacy_InventoryLocation] ([TenantId], [FacilityId], [ParentLocationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Pharmacy_ReorderPolicy_TenantId_FacilityId_BatchStockId] ON [pharmacy].[Pharmacy_ReorderPolicy] ([TenantId], [FacilityId], [BatchStockId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Pharmacy_SalesReturn_TenantId_FacilityId_ReturnNo] ON [pharmacy].[Pharmacy_SalesReturn] ([TenantId], [FacilityId], [ReturnNo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    CREATE INDEX [IX_Pharmacy_SalesReturnItem_TenantId_FacilityId_SalesReturnId] ON [pharmacy].[Pharmacy_SalesReturnItem] ([TenantId], [FacilityId], [SalesReturnId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260331192050_InitialUnifiedSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260331192050_InitialUnifiedSchema', N'8.0.11');
END;
GO

COMMIT;
GO

