SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  PHARMACY Management (Batch-wise critical) - Microservice-ready module tables
  Depends on:
    - Root hierarchy: Facility
    - Shared domain: Unit, ReferenceData*, Patient, Doctor
    - HMS: HMS_Prescription, HMS_PrescriptionItems, HMS_BillingItems
*/

/* =======================================================================
   Medicine Master
   ======================================================================= */
CREATE TABLE dbo.MedicineCategory (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_MedicineCategory_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_MedicineCategory_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_MedicineCategory_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_MedicineCategory_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_MedicineCategory_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_MedicineCategory_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CategoryCode NVARCHAR(80) NOT NULL,
    CategoryName NVARCHAR(250) NOT NULL,
    Description NVARCHAR(500) NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_MedicineCategory PRIMARY KEY (Id),
    CONSTRAINT UQ_MedicineCategory_Tenant_Code UNIQUE (TenantId, CategoryCode),
    CONSTRAINT UQ_MedicineCategory_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_MedicineCategory_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

CREATE TABLE dbo.Manufacturer (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Manufacturer_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Manufacturer_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Manufacturer_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Manufacturer_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Manufacturer_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Manufacturer_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ManufacturerCode NVARCHAR(80) NULL,
    ManufacturerName NVARCHAR(250) NOT NULL,
    CountryCode NVARCHAR(10) NULL,

    CONSTRAINT PK_Manufacturer PRIMARY KEY (Id),
    CONSTRAINT UQ_Manufacturer_Tenant_Name UNIQUE (TenantId, ManufacturerName),
    CONSTRAINT UQ_Manufacturer_TenantId_Id UNIQUE (TenantId, Id)
);
GO

CREATE TABLE dbo.Composition (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Composition_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Composition_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Composition_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Composition_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Composition_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Composition_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CompositionName NVARCHAR(250) NOT NULL,
    CompositionCode NVARCHAR(120) NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_Composition PRIMARY KEY (Id),
    CONSTRAINT UQ_Composition_Tenant_Name UNIQUE (TenantId, CompositionName),
    CONSTRAINT UQ_Composition_TenantId_Id UNIQUE (TenantId, Id)
);
GO

CREATE TABLE dbo.Medicine (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Medicine_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Medicine_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Medicine_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Medicine_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Medicine_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Medicine_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    MedicineCode NVARCHAR(80) NOT NULL,
    MedicineName NVARCHAR(300) NOT NULL,

    CategoryId BIGINT NOT NULL,
    ManufacturerId BIGINT NULL,

    Strength NVARCHAR(120) NULL,
    DefaultUnitId BIGINT NULL,

    FormReferenceValueId BIGINT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_Medicine PRIMARY KEY (Id),
    CONSTRAINT UQ_Medicine_Tenant_Code UNIQUE (TenantId, MedicineCode),
    CONSTRAINT UQ_Medicine_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_Medicine_Category FOREIGN KEY (TenantId, CategoryId)
        REFERENCES dbo.MedicineCategory(TenantId, Id),
    CONSTRAINT FK_Medicine_Manufacturer FOREIGN KEY (TenantId, ManufacturerId)
        REFERENCES dbo.Manufacturer(TenantId, Id),
    CONSTRAINT FK_Medicine_DefaultUnit FOREIGN KEY (TenantId, DefaultUnitId)
        REFERENCES dbo.Unit(TenantId, Id),
    CONSTRAINT FK_Medicine_Form FOREIGN KEY (TenantId, FormReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.MedicineComposition (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_MedicineComposition_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_MedicineComposition_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_MedicineComposition_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_MedicineComposition_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_MedicineComposition_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_MedicineComposition_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    MedicineId BIGINT NOT NULL,
    CompositionId BIGINT NOT NULL,
    Amount DECIMAL(18,4) NULL,
    UnitId BIGINT NULL,

    CONSTRAINT PK_MedicineComposition PRIMARY KEY (Id),
    CONSTRAINT UQ_MedicineComposition_Tenant_Med_Comp UNIQUE (TenantId, MedicineId, CompositionId),
    CONSTRAINT UQ_MedicineComposition_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_MedicineComposition_Medicine FOREIGN KEY (TenantId, MedicineId)
        REFERENCES dbo.Medicine(TenantId, Id),
    CONSTRAINT FK_MedicineComposition_Composition FOREIGN KEY (TenantId, CompositionId)
        REFERENCES dbo.Composition(TenantId, Id),
    CONSTRAINT FK_MedicineComposition_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

/* =======================================================================
   Batch-wise Inventory (critical)
   ======================================================================= */
CREATE TABLE dbo.MedicineBatch (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_MedicineBatch_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_MedicineBatch_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_MedicineBatch_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_MedicineBatch_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_MedicineBatch_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_MedicineBatch_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    MedicineId BIGINT NOT NULL,
    BatchNo NVARCHAR(120) NOT NULL,

    ExpiryDate DATE NULL,
    MRP DECIMAL(18,4) NULL,
    PurchaseRate DECIMAL(18,4) NULL,
    ManufacturingDate DATE NULL,

    CONSTRAINT PK_MedicineBatch PRIMARY KEY (Id),
    CONSTRAINT UQ_MedicineBatch_Tenant_Med_Batch UNIQUE (TenantId, MedicineId, BatchNo),
    CONSTRAINT UQ_MedicineBatch_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_MedicineBatch_Medicine FOREIGN KEY (TenantId, MedicineId)
        REFERENCES dbo.Medicine(TenantId, Id)
);
GO

/* BatchStock (per batch stock tracking, per facility/warehouse) */
CREATE TABLE dbo.BatchStock (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_BatchStock_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_BatchStock_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_BatchStock_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_BatchStock_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_BatchStock_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_BatchStock_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    MedicineBatchId BIGINT NOT NULL,
    CurrentQty DECIMAL(18,4) NOT NULL CONSTRAINT DF_BatchStock_CurrentQty DEFAULT (0),
    ReservedQty DECIMAL(18,4) NOT NULL CONSTRAINT DF_BatchStock_ReservedQty DEFAULT (0),
    AvailableQty DECIMAL(18,4) NOT NULL CONSTRAINT DF_BatchStock_AvailableQty DEFAULT (0),

    ReorderLevelQty DECIMAL(18,4) NULL,
    LastUpdatedOn DATETIME2(7) NULL,

    CONSTRAINT PK_BatchStock PRIMARY KEY (Id),
    CONSTRAINT UQ_BatchStock_TenantFacility_Batch UNIQUE (TenantId, FacilityId, MedicineBatchId),
    CONSTRAINT UQ_BatchStock_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_BatchStock_Batch FOREIGN KEY (TenantId, MedicineBatchId)
        REFERENCES dbo.MedicineBatch(TenantId, Id)
);
GO

/* StockLedger (movement-level tracking, audit heavy) */
CREATE TABLE dbo.StockLedger (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_StockLedger_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_StockLedger_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockLedger_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_StockLedger_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockLedger_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_StockLedger_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    MedicineBatchId BIGINT NOT NULL,
    LedgerTypeReferenceValueId BIGINT NULL,

    TransactionOn DATETIME2(7) NOT NULL,
    QuantityDelta DECIMAL(18,4) NOT NULL,
    BeforeQty DECIMAL(18,4) NOT NULL,
    AfterQty DECIMAL(18,4) NOT NULL,

    UnitCost DECIMAL(18,4) NULL,
    TotalCost DECIMAL(18,4) NULL,

    SourceReference NVARCHAR(200) NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_StockLedger PRIMARY KEY (Id),
    CONSTRAINT UQ_StockLedger_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_StockLedger_Batch FOREIGN KEY (TenantId, MedicineBatchId)
        REFERENCES dbo.MedicineBatch(TenantId, Id),
    CONSTRAINT FK_StockLedger_LedgerType FOREIGN KEY (TenantId, LedgerTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   Procurement: PurchaseOrder / Items / GRN
   ======================================================================= */
CREATE TABLE dbo.PurchaseOrder (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PurchaseOrder_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PurchaseOrder_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PurchaseOrder_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PurchaseOrder_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PurchaseOrder_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PurchaseOrder_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PurchaseOrderNo NVARCHAR(60) NOT NULL,
    SupplierName NVARCHAR(250) NOT NULL,

    OrderDate DATETIME2(7) NOT NULL,
    ExpectedOn DATETIME2(7) NULL,

    StatusReferenceValueId BIGINT NOT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_PurchaseOrder PRIMARY KEY (Id),
    CONSTRAINT UQ_PurchaseOrder_TenantFacility_No UNIQUE (TenantId, FacilityId, PurchaseOrderNo),
    CONSTRAINT UQ_PurchaseOrder_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_PurchaseOrder_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.PurchaseOrderItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PurchaseOrderItems_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PurchaseOrderItems_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PurchaseOrderItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PurchaseOrderItems_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PurchaseOrderItems_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PurchaseOrderItems_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PurchaseOrderId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_PurchaseOrderItems_Line DEFAULT (0),

    MedicineId BIGINT NOT NULL,
    QuantityOrdered DECIMAL(18,4) NOT NULL,
    UnitId BIGINT NULL,
    PurchaseRate DECIMAL(18,4) NULL,

    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_PurchaseOrderItems PRIMARY KEY (Id),
    CONSTRAINT UQ_PurchaseOrderItems_TenantFacility_Line UNIQUE (TenantId, FacilityId, PurchaseOrderId, LineNum),
    CONSTRAINT UQ_PurchaseOrderItems_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_POItems_PurchaseOrder FOREIGN KEY (TenantId, FacilityId, PurchaseOrderId)
        REFERENCES dbo.PurchaseOrder(TenantId, FacilityId, Id),
    CONSTRAINT FK_POItems_Medicine FOREIGN KEY (TenantId, MedicineId)
        REFERENCES dbo.Medicine(TenantId, Id),
    CONSTRAINT FK_POItems_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

CREATE TABLE dbo.GoodsReceipt (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_GoodsReceipt_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_GoodsReceipt_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_GoodsReceipt_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_GoodsReceipt_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_GoodsReceipt_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_GoodsReceipt_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    GoodsReceiptNo NVARCHAR(60) NOT NULL,
    PurchaseOrderId BIGINT NOT NULL,

    ReceivedOn DATETIME2(7) NOT NULL,
    ReceivedByDoctorId BIGINT NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_GoodsReceipt PRIMARY KEY (Id),
    CONSTRAINT UQ_GoodsReceipt_TenantFacility_No UNIQUE (TenantId, FacilityId, GoodsReceiptNo),
    CONSTRAINT UQ_GoodsReceipt_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_GR_PurchaseOrder FOREIGN KEY (TenantId, FacilityId, PurchaseOrderId)
        REFERENCES dbo.PurchaseOrder(TenantId, FacilityId, Id),
    CONSTRAINT FK_GR_ByDoctor FOREIGN KEY (TenantId, ReceivedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_GR_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.GoodsReceiptItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_GoodsReceiptItems_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_GoodsReceiptItems_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_GoodsReceiptItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_GoodsReceiptItems_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_GoodsReceiptItems_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_GoodsReceiptItems_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    GoodsReceiptId BIGINT NOT NULL,
    PurchaseOrderItemId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_GoodsReceiptItems_Line DEFAULT (0),

    MedicineId BIGINT NOT NULL,
    MedicineBatchId BIGINT NOT NULL,
    QuantityReceived DECIMAL(18,4) NOT NULL,
    UnitId BIGINT NULL,

    PurchaseRate DECIMAL(18,4) NULL,
    ExpiryDate DATE NULL,
    MRP DECIMAL(18,4) NULL,

    CONSTRAINT PK_GoodsReceiptItems PRIMARY KEY (Id),
    CONSTRAINT UQ_GoodsReceiptItems_TenantFacility_Line UNIQUE (TenantId, FacilityId, GoodsReceiptId, LineNum),
    CONSTRAINT UQ_GoodsReceiptItems_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_GRItems_GR FOREIGN KEY (TenantId, FacilityId, GoodsReceiptId)
        REFERENCES dbo.GoodsReceipt(TenantId, FacilityId, Id),
    CONSTRAINT FK_GRItems_POItem FOREIGN KEY (TenantId, FacilityId, PurchaseOrderItemId)
        REFERENCES dbo.PurchaseOrderItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_GRItems_Medicine FOREIGN KEY (TenantId, MedicineId)
        REFERENCES dbo.Medicine(TenantId, Id),
    CONSTRAINT FK_GRItems_Batch FOREIGN KEY (TenantId, MedicineBatchId)
        REFERENCES dbo.MedicineBatch(TenantId, Id),
    CONSTRAINT FK_GRItems_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

/* =======================================================================
   Sales: PharmacySales / Items / PrescriptionMapping
   ======================================================================= */
CREATE TABLE dbo.PharmacySales (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PharmacySales_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PharmacySales_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PharmacySales_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PharmacySales_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PharmacySales_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PharmacySales_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SalesNo NVARCHAR(60) NOT NULL,

    PatientId BIGINT NOT NULL,
    VisitId BIGINT NULL,

    PrescriptionId BIGINT NULL,   -- Prescription -> PharmacySales linkage
    DoctorId BIGINT NULL,

    SalesDate DATETIME2(7) NOT NULL,
    StatusReferenceValueId BIGINT NOT NULL,

    CurrencyCode NVARCHAR(3) NULL,
    PaymentTotal DECIMAL(18,4) NULL,

    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_PharmacySales PRIMARY KEY (Id),
    CONSTRAINT UQ_PharmacySales_TenantFacility_No UNIQUE (TenantId, FacilityId, SalesNo),
    CONSTRAINT UQ_PharmacySales_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_PharmacySales_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_PharmacySales_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_PharmacySales_Doctor FOREIGN KEY (TenantId, DoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_PharmacySales_Prescription FOREIGN KEY (TenantId, FacilityId, PrescriptionId)
        REFERENCES dbo.HMS_Prescription(TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.PharmacySalesItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PharmacySalesItems_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PharmacySalesItems_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PharmacySalesItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PharmacySalesItems_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PharmacySalesItems_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PharmacySalesItems_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PharmacySalesId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_PharmacySalesItems_Line DEFAULT (0),

    MedicineId BIGINT NOT NULL,
    MedicineBatchId BIGINT NOT NULL,

    PrescriptionItemId BIGINT NULL, -- optional HMS item linkage

    QuantitySold DECIMAL(18,4) NOT NULL,
    UnitId BIGINT NULL,
    UnitPrice DECIMAL(18,4) NULL,
    LineTotal DECIMAL(18,4) NULL,
    DispensedOn DATETIME2(7) NULL,

    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_PharmacySalesItems PRIMARY KEY (Id),
    CONSTRAINT UQ_PharmacySalesItems_TenantFacility_Line UNIQUE (TenantId, FacilityId, PharmacySalesId, LineNum),
    CONSTRAINT UQ_PharmacySalesItems_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_SalesItems_Sales FOREIGN KEY (TenantId, FacilityId, PharmacySalesId)
        REFERENCES dbo.PharmacySales(TenantId, FacilityId, Id),
    CONSTRAINT FK_SalesItems_Medicine FOREIGN KEY (TenantId, MedicineId)
        REFERENCES dbo.Medicine(TenantId, Id),
    CONSTRAINT FK_SalesItems_Batch FOREIGN KEY (TenantId, MedicineBatchId)
        REFERENCES dbo.MedicineBatch(TenantId, Id),
    CONSTRAINT FK_SalesItems_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id),
    CONSTRAINT FK_SalesItems_PrescriptionItem FOREIGN KEY (TenantId, FacilityId, PrescriptionItemId)
        REFERENCES dbo.HMS_PrescriptionItems(TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.PrescriptionMapping (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PrescriptionMapping_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PrescriptionMapping_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PrescriptionMapping_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PrescriptionMapping_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PrescriptionMapping_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PrescriptionMapping_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PrescriptionId BIGINT NOT NULL,
    PharmacySalesId BIGINT NOT NULL,
    PrescriptionItemId BIGINT NULL,
    PharmacySalesItemId BIGINT NULL,

    MappedQty DECIMAL(18,4) NULL,
    MappingNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_PrescriptionMapping PRIMARY KEY (Id),
    CONSTRAINT UQ_PrescriptionMapping_TenantFacility_Pres_Sales UNIQUE (TenantId, FacilityId, PrescriptionId, PharmacySalesId),
    CONSTRAINT UQ_PrescriptionMapping_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_PrescriptionMapping_Prescription FOREIGN KEY (TenantId, FacilityId, PrescriptionId)
        REFERENCES dbo.HMS_Prescription(TenantId, FacilityId, Id),
    CONSTRAINT FK_PrescriptionMapping_Sales FOREIGN KEY (TenantId, FacilityId, PharmacySalesId)
        REFERENCES dbo.PharmacySales(TenantId, FacilityId, Id),
    CONSTRAINT FK_PrescriptionMapping_PresItem FOREIGN KEY (TenantId, FacilityId, PrescriptionItemId)
        REFERENCES dbo.HMS_PrescriptionItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_PrescriptionMapping_SalesItem FOREIGN KEY (TenantId, FacilityId, PharmacySalesItemId)
        REFERENCES dbo.PharmacySalesItems(TenantId, FacilityId, Id)
);
GO

/* =======================================================================
   Inventory Control: Adjustments, Transfers, ExpiryTracking
   ======================================================================= */
CREATE TABLE dbo.StockAdjustment (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_StockAdjustment_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_StockAdjustment_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockAdjustment_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_StockAdjustment_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockAdjustment_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_StockAdjustment_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AdjustmentNo NVARCHAR(60) NOT NULL,
    AdjustmentOn DATETIME2(7) NOT NULL,
    AdjustmentTypeReferenceValueId BIGINT NULL,

    PerformedByDoctorId BIGINT NULL,
    ReasonNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_StockAdjustment PRIMARY KEY (Id),
    CONSTRAINT UQ_StockAdjustment_TenantFacility_No UNIQUE (TenantId, FacilityId, AdjustmentNo),
    CONSTRAINT UQ_StockAdjustment_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_StockAdjustment_Type FOREIGN KEY (TenantId, AdjustmentTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_StockAdjustment_PerformedBy FOREIGN KEY (TenantId, PerformedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

CREATE TABLE dbo.StockAdjustmentItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_StockAdjustmentItems_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_StockAdjustmentItems_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockAdjustmentItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_StockAdjustmentItems_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockAdjustmentItems_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_StockAdjustmentItems_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    StockAdjustmentId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_StockAdjustmentItems_Line DEFAULT (0),
    MedicineBatchId BIGINT NOT NULL,
    QuantityDelta DECIMAL(18,4) NOT NULL,
    UnitCost DECIMAL(18,4) NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_StockAdjustmentItems PRIMARY KEY (Id),
    CONSTRAINT UQ_StockAdjustmentItems_TenantFacility_Line UNIQUE (TenantId, FacilityId, StockAdjustmentId, LineNum),
    CONSTRAINT UQ_StockAdjustmentItems_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_StockAdjustmentItems_Header FOREIGN KEY (TenantId, FacilityId, StockAdjustmentId)
        REFERENCES dbo.StockAdjustment(TenantId, FacilityId, Id),
    CONSTRAINT FK_StockAdjustmentItems_Batch FOREIGN KEY (TenantId, MedicineBatchId)
        REFERENCES dbo.MedicineBatch(TenantId, Id)
);
GO

CREATE TABLE dbo.StockTransfer (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,     -- source facility in this record
    IsActive BIT NOT NULL CONSTRAINT DF_StockTransfer_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_StockTransfer_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockTransfer_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_StockTransfer_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockTransfer_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_StockTransfer_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TransferNo NVARCHAR(60) NOT NULL,
    FromFacilityId BIGINT NOT NULL,
    ToFacilityId BIGINT NOT NULL,
    TransferOn DATETIME2(7) NOT NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_StockTransfer PRIMARY KEY (Id),
    CONSTRAINT UQ_StockTransfer_TenantFacility_No UNIQUE (TenantId, FacilityId, TransferNo),
    CONSTRAINT UQ_StockTransfer_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_StockTransfer_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.StockTransferItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_StockTransferItems_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_StockTransferItems_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockTransferItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_StockTransferItems_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_StockTransferItems_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_StockTransferItems_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    StockTransferId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_StockTransferItems_Line DEFAULT (0),
    MedicineBatchId BIGINT NOT NULL,
    Quantity DECIMAL(18,4) NOT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_StockTransferItems PRIMARY KEY (Id),
    CONSTRAINT UQ_StockTransferItems_TenantFacility_Line UNIQUE (TenantId, FacilityId, StockTransferId, LineNum),
    CONSTRAINT UQ_StockTransferItems_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_StockTransferItems_Header FOREIGN KEY (TenantId, FacilityId, StockTransferId)
        REFERENCES dbo.StockTransfer(TenantId, FacilityId, Id),
    CONSTRAINT FK_StockTransferItems_Batch FOREIGN KEY (TenantId, MedicineBatchId)
        REFERENCES dbo.MedicineBatch(TenantId, Id)
);
GO

CREATE TABLE dbo.ExpiryTracking (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ExpiryTracking_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_ExpiryTracking_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ExpiryTracking_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_ExpiryTracking_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ExpiryTracking_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_ExpiryTracking_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    MedicineBatchId BIGINT NOT NULL,
    ExpiryDate DATE NOT NULL,
    DaysToExpiry INT NULL,
    ExpiryAlertStatusReferenceValueId BIGINT NULL,
    LastReviewedOn DATETIME2(7) NULL,
    ReviewedByDoctorId BIGINT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_ExpiryTracking PRIMARY KEY (Id),
    CONSTRAINT UQ_ExpiryTracking_TenantFacility_BatchExp UNIQUE (TenantId, FacilityId, MedicineBatchId, ExpiryDate),
    CONSTRAINT UQ_ExpiryTracking_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_ExpiryTracking_Batch FOREIGN KEY (TenantId, MedicineBatchId)
        REFERENCES dbo.MedicineBatch(TenantId, Id),
    CONSTRAINT FK_ExpiryTracking_Status FOREIGN KEY (TenantId, ExpiryAlertStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_ExpiryTracking_ReviewedBy FOREIGN KEY (TenantId, ReviewedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   Indexing (Pharmacy hot paths)
   ======================================================================= */
CREATE NONCLUSTERED INDEX IX_BatchStock_TenantFacility_Available
ON dbo.BatchStock (TenantId, FacilityId, AvailableQty)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_StockLedger_TenantFacility_Batch_On
ON dbo.StockLedger (TenantId, FacilityId, MedicineBatchId, TransactionOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_PharmacySales_TenantFacility_DateStatus
ON dbo.PharmacySales (TenantId, FacilityId, SalesDate DESC, StatusReferenceValueId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_PharmacySalesItems_TenantFacility_Batch
ON dbo.PharmacySalesItems (TenantId, FacilityId, MedicineBatchId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_ExpiryTracking_TenantFacility_Expiry
ON dbo.ExpiryTracking (TenantId, FacilityId, ExpiryDate)
WHERE IsDeleted = 0;
GO

/* =======================================================================
   Cross-module FK attachments (ALTER)
   ======================================================================= */

-- 1) Attach HMS_PrescriptionItems.MedicineId -> Pharmacy.Medicine (Medicine is in this script)
IF COL_LENGTH('dbo.HMS_PrescriptionItems', 'MedicineId') IS NOT NULL
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.HMS_PrescriptionItems WITH CHECK ADD CONSTRAINT FK_HMS_PrescriptionItems_Medicine
            FOREIGN KEY (TenantId, MedicineId)
            REFERENCES dbo.Medicine(TenantId, Id);
    END TRY
    BEGIN CATCH
    END CATCH;
END;
GO

-- 2) Attach HMS_BillingItems.PharmacySalesId -> Pharmacy.PharmacySales (for billing integration)
IF COL_LENGTH('dbo.HMS_BillingItems', 'PharmacySalesId') IS NOT NULL
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.HMS_BillingItems WITH CHECK ADD CONSTRAINT FK_HMS_BillingItems_PharmacySales
            FOREIGN KEY (TenantId, FacilityId, PharmacySalesId)
            REFERENCES dbo.PharmacySales(TenantId, FacilityId, Id);
    END TRY
    BEGIN CATCH
    END CATCH;
END;
GO

/* =======================================================================
   Suggested Partitioning (optional)
   - StockLedger: partition by TransactionOn (monthly RANGE)
   - ExpiryTracking reviewed activity: optional by ExpiryDate
   - PharmacySales: partition by SalesDate / CreatedOn as needed
   ======================================================================= */

