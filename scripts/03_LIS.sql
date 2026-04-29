SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  LIS (Laboratory Information System)
  Depends on:
    - Shared domain: Patient, Doctor, Unit, ReferenceData
    - Root hierarchy: Facility, Department
    - HMS: HMS_Prescription (for Prescription -> LabOrder linkage)

  Cross-module FK:
    - LabOrder -> Billing: attaches HMS_BillingItems.LabOrderId FK via ALTER TABLE at the end.
*/

/* =======================================================================
   LIS Test Catalog (catalog-level; FacilityId nullable)
   ======================================================================= */
CREATE TABLE dbo.LIS_TestCategory (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_TestCategory_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_TestCategory_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestCategory_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestCategory_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestCategory_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestCategory_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CategoryCode NVARCHAR(80) NOT NULL,
    CategoryName NVARCHAR(250) NOT NULL,
    ParentCategoryId BIGINT NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_LIS_TestCategory PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_TestCategory_Tenant_Code UNIQUE (TenantId, CategoryCode),
    CONSTRAINT UQ_LIS_TestCategory_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_LIS_TestCategory_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_LIS_TestCategory_Parent FOREIGN KEY (TenantId, ParentCategoryId)
        REFERENCES dbo.LIS_TestCategory(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_SampleType (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_SampleType_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_SampleType_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleType_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleType_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleType_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleType_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SampleTypeCode NVARCHAR(80) NOT NULL,
    SampleTypeName NVARCHAR(250) NOT NULL,
    Description NVARCHAR(500) NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_LIS_SampleType PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_SampleType_Tenant_Code UNIQUE (TenantId, SampleTypeCode),
    CONSTRAINT UQ_LIS_SampleType_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_LIS_SampleType_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

CREATE TABLE dbo.LIS_TestMaster (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_TestMaster_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_TestMaster_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestMaster_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestMaster_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestMaster_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestMaster_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CategoryId BIGINT NOT NULL,
    SampleTypeId BIGINT NULL,

    TestCode NVARCHAR(80) NOT NULL,
    TestName NVARCHAR(250) NOT NULL,
    TestDescription NVARCHAR(1000) NULL,

    DefaultUnitId BIGINT NULL,
    IsQuantitative BIT NOT NULL CONSTRAINT DF_LIS_TestMaster_IsQuantitative DEFAULT (1),

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_LIS_TestMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_TestMaster_Tenant_Code UNIQUE (TenantId, TestCode),
    CONSTRAINT UQ_LIS_TestMaster_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_LIS_TestMaster_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_LIS_TestMaster_Category FOREIGN KEY (TenantId, CategoryId)
        REFERENCES dbo.LIS_TestCategory(TenantId, Id),
    CONSTRAINT FK_LIS_TestMaster_SampleType FOREIGN KEY (TenantId, SampleTypeId)
        REFERENCES dbo.LIS_SampleType(TenantId, Id),
    CONSTRAINT FK_LIS_TestMaster_DefaultUnit FOREIGN KEY (TenantId, DefaultUnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_TestParameters (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_TestParameters_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_TestParameters_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestParameters_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestParameters_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestParameters_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestParameters_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestMasterId BIGINT NOT NULL,
    ParameterCode NVARCHAR(100) NOT NULL,
    ParameterName NVARCHAR(300) NOT NULL,
    DisplayOrder INT NOT NULL CONSTRAINT DF_LIS_TestParameters_DisplayOrder DEFAULT (0),

    IsNumeric BIT NOT NULL CONSTRAINT DF_LIS_TestParameters_IsNumeric DEFAULT (1),
    UnitId BIGINT NULL,
    ParameterNotes NVARCHAR(500) NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_LIS_TestParameters PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_TestParameters_Tenant_TestParamCode UNIQUE (TenantId, TestMasterId, ParameterCode),
    CONSTRAINT UQ_LIS_TestParameters_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_LIS_TestParameters_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_LIS_TestParameters_TestMaster FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id),
    CONSTRAINT FK_LIS_TestParameters_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_TestReferenceRanges (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_TestReferenceRanges_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_TestReferenceRanges_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestReferenceRanges_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestReferenceRanges_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestReferenceRanges_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestReferenceRanges_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestParameterId BIGINT NOT NULL,
    SexReferenceValueId BIGINT NULL,
    AgeFromYears INT NULL,
    AgeToYears INT NULL,
    ReferenceRangeTypeReferenceValueId BIGINT NULL,

    MinValue DECIMAL(18,4) NULL,
    MaxValue DECIMAL(18,4) NULL,
    RangeUnitId BIGINT NULL,
    RangeNotes NVARCHAR(800) NULL,
    EffectiveFromDate DATE NULL,
    EffectiveToDate DATE NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_LIS_TestReferenceRanges PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_TestReferenceRanges_Tenant_Param_RangeType UNIQUE (TenantId, TestParameterId, SexReferenceValueId, AgeFromYears, AgeToYears, ReferenceRangeTypeReferenceValueId, MinValue, MaxValue, EffectiveFromDate),
    CONSTRAINT UQ_LIS_TestReferenceRanges_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_LIS_TestReferenceRanges_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_LIS_TestReferenceRanges_TestParameter FOREIGN KEY (TenantId, TestParameterId)
        REFERENCES dbo.LIS_TestParameters(TenantId, Id),
    CONSTRAINT FK_LIS_TestReferenceRanges_Sex FOREIGN KEY (TenantId, SexReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_TestReferenceRanges_RangeType FOREIGN KEY (TenantId, ReferenceRangeTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_TestReferenceRanges_RangeUnit FOREIGN KEY (TenantId, RangeUnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

/* =======================================================================
   LabOrder (header)
   ======================================================================= */
CREATE TABLE dbo.LIS_LabOrder (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_LabOrder_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_LabOrder_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_LabOrder_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_LabOrder_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_LabOrder_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_LabOrder_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderNo NVARCHAR(60) NOT NULL,

    PatientId BIGINT NOT NULL,
    FacilityDepartmentId BIGINT NULL,   -- optional quick linkage for sorting by department
    VisitId BIGINT NULL,
    PrescriptionId BIGINT NULL,          -- Prescription -> LabOrder linkage

    OrderingDoctorId BIGINT NULL,
    DepartmentId BIGINT NULL,

    OrderedOn DATETIME2(7) NOT NULL,
    OrderStatusReferenceValueId BIGINT NOT NULL,
    PriorityReferenceValueId BIGINT NULL,

    ClinicalNotes NVARCHAR(MAX) NULL,
    RequestedCollectionOn DATETIME2(7) NULL,

    CONSTRAINT PK_LIS_LabOrder PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_LabOrder_Tenant_Facility_No UNIQUE (TenantId, FacilityId, LabOrderNo),
    CONSTRAINT UQ_LIS_LabOrder_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_LabOrder_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_LIS_LabOrder_Prescription FOREIGN KEY (TenantId, FacilityId, PrescriptionId)
        REFERENCES dbo.HMS_Prescription(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_LabOrder_Status FOREIGN KEY (TenantId, OrderStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_LabOrder_Priority FOREIGN KEY (TenantId, PriorityReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_LabOrder_Doctor FOREIGN KEY (TenantId, OrderingDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_LIS_LabOrder_Department FOREIGN KEY (TenantId, DepartmentId)
        REFERENCES dbo.Department(TenantId, Id)
);
GO

/* =======================================================================
   LabOrderItems
   ======================================================================= */
CREATE TABLE dbo.LIS_LabOrderItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_LabOrderItems_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_LabOrderItems_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_LabOrderItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_LabOrderItems_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_LabOrderItems_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_LabOrderItems_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderId BIGINT NOT NULL,
    TestMasterId BIGINT NOT NULL,
    SampleTypeId BIGINT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_LIS_LabOrderItems_Line DEFAULT (0),
    RequestedOn DATETIME2(7) NOT NULL,
    OrderItemStatusReferenceValueId BIGINT NOT NULL,

    SpecimenVolume DECIMAL(18,4) NULL,
    SpecimenVolumeUnitId BIGINT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_LIS_LabOrderItems PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_LabOrderItems_Tenant_Facility_Order_Line UNIQUE (TenantId, FacilityId, LabOrderId, LineNum),
    CONSTRAINT UQ_LIS_LabOrderItems_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_LabOrderItems_Order FOREIGN KEY (TenantId, FacilityId, LabOrderId)
        REFERENCES dbo.LIS_LabOrder(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_LabOrderItems_TestMaster FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id),
    CONSTRAINT FK_LIS_LabOrderItems_SampleType FOREIGN KEY (TenantId, SampleTypeId)
        REFERENCES dbo.LIS_SampleType(TenantId, Id),
    CONSTRAINT FK_LIS_LabOrderItems_Status FOREIGN KEY (TenantId, OrderItemStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_LabOrderItems_VolUnit FOREIGN KEY (TenantId, SpecimenVolumeUnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

/* =======================================================================
   OrderStatusHistory
   ======================================================================= */
CREATE TABLE dbo.LIS_OrderStatusHistory (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_OrderStatusHistory_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_OrderStatusHistory_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_OrderStatusHistory_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_OrderStatusHistory_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_OrderStatusHistory_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_OrderStatusHistory_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderId BIGINT NOT NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    StatusOn DATETIME2(7) NOT NULL,
    StatusNote NVARCHAR(1000) NULL,
    ChangedByDoctorId BIGINT NULL,

    CONSTRAINT PK_LIS_OrderStatusHistory PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_OrderStatusHistory_TenantFacility UNIQUE (TenantId, FacilityId, LabOrderId, StatusOn, StatusReferenceValueId),
    CONSTRAINT UQ_LIS_OrderStatusHistory_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_OrderStatusHistory_Order FOREIGN KEY (TenantId, FacilityId, LabOrderId)
        REFERENCES dbo.LIS_LabOrder(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_OrderStatusHistory_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_OrderStatusHistory_ChangedByDoctor FOREIGN KEY (TenantId, ChangedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   SampleCollection
   ======================================================================= */
CREATE TABLE dbo.LIS_SampleCollection (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_SampleCollection_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_SampleCollection_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleCollection_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleCollection_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleCollection_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleCollection_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderItemId BIGINT NOT NULL,
    SampleTypeId BIGINT NOT NULL,

    CollectedOn DATETIME2(7) NOT NULL,
    CollectedByDoctorId BIGINT NULL,
    CollectionDepartmentId BIGINT NULL,

    CollectedQuantity DECIMAL(18,4) NULL,
    CollectedQuantityUnitId BIGINT NULL,
    CollectionNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_LIS_SampleCollection PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_SampleCollection_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_SampleCollection_OrderItem FOREIGN KEY (TenantId, FacilityId, LabOrderItemId)
        REFERENCES dbo.LIS_LabOrderItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_SampleCollection_SampleType FOREIGN KEY (TenantId, SampleTypeId)
        REFERENCES dbo.LIS_SampleType(TenantId, Id),
    CONSTRAINT FK_LIS_SampleCollection_CollectedByDoctor FOREIGN KEY (TenantId, CollectedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_LIS_SampleCollection_CollectionDept FOREIGN KEY (TenantId, CollectionDepartmentId)
        REFERENCES dbo.Department(TenantId, Id),
    CONSTRAINT FK_LIS_SampleCollection_QuantityUnit FOREIGN KEY (TenantId, CollectedQuantityUnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

/* =======================================================================
   SampleTracking
   ======================================================================= */
CREATE TABLE dbo.LIS_SampleTracking (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_SampleTracking_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_SampleTracking_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleTracking_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleTracking_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleTracking_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleTracking_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SampleCollectionId BIGINT NOT NULL,
    TrackingNo NVARCHAR(120) NOT NULL,
    TrackingEventTypeReferenceValueId BIGINT NULL,
    TrackingStatusReferenceValueId BIGINT NULL,
    LocationDepartmentId BIGINT NULL,

    TrackedOn DATETIME2(7) NOT NULL,
    ScannedByDoctorId BIGINT NULL,
    TrackingNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_LIS_SampleTracking PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_SampleTracking_TenantFacility_TrackingNo UNIQUE (TenantId, FacilityId, TrackingNo),
    CONSTRAINT UQ_LIS_SampleTracking_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_SampleTracking_Collection FOREIGN KEY (TenantId, FacilityId, SampleCollectionId)
        REFERENCES dbo.LIS_SampleCollection(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_SampleTracking_EventType FOREIGN KEY (TenantId, TrackingEventTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_SampleTracking_Status FOREIGN KEY (TenantId, TrackingStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_SampleTracking_LocationDept FOREIGN KEY (TenantId, LocationDepartmentId)
        REFERENCES dbo.Department(TenantId, Id),
    CONSTRAINT FK_LIS_SampleTracking_ScannedBy FOREIGN KEY (TenantId, ScannedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   LabResults (parameter-wise granular)
   ======================================================================= */
CREATE TABLE dbo.LIS_LabResults (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_LabResults_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_LabResults_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_LabResults_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_LabResults_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_LabResults_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_LabResults_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderItemId BIGINT NOT NULL,
    TestParameterId BIGINT NOT NULL,

    ResultNumeric DECIMAL(18,4) NULL,
    ResultText NVARCHAR(MAX) NULL,
    ResultUnitId BIGINT NULL,

    IsAbnormal BIT NOT NULL CONSTRAINT DF_LIS_LabResults_IsAbnormal DEFAULT (0),
    AbnormalFlagReferenceValueId BIGINT NULL,
    Notes NVARCHAR(1000) NULL,

    MeasuredOn DATETIME2(7) NULL,
    ResultStatusReferenceValueId BIGINT NOT NULL,
    EnteredByDoctorId BIGINT NULL,

    CONSTRAINT PK_LIS_LabResults PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_LabResults_TenantFacility_Item_Param UNIQUE (TenantId, FacilityId, LabOrderItemId, TestParameterId),
    CONSTRAINT UQ_LIS_LabResults_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_LabResults_OrderItem FOREIGN KEY (TenantId, FacilityId, LabOrderItemId)
        REFERENCES dbo.LIS_LabOrderItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_LabResults_TestParameter FOREIGN KEY (TenantId, TestParameterId)
        REFERENCES dbo.LIS_TestParameters(TenantId, Id),
    CONSTRAINT FK_LIS_LabResults_Unit FOREIGN KEY (TenantId, ResultUnitId)
        REFERENCES dbo.Unit(TenantId, Id),
    CONSTRAINT FK_LIS_LabResults_AbnormalFlag FOREIGN KEY (TenantId, AbnormalFlagReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_LabResults_Status FOREIGN KEY (TenantId, ResultStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_LabResults_EnteredByDoctor FOREIGN KEY (TenantId, EnteredByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   ResultApproval
   ======================================================================= */
CREATE TABLE dbo.LIS_ResultApproval (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_ResultApproval_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_ResultApproval_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ResultApproval_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ResultApproval_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ResultApproval_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ResultApproval_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabResultsId BIGINT NOT NULL,
    ApprovalStatusReferenceValueId BIGINT NOT NULL,
    ApprovedByDoctorId BIGINT NOT NULL,
    ApprovedOn DATETIME2(7) NOT NULL,
    ApprovalNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_LIS_ResultApproval PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_ResultApproval_TenantFacility_Result UNIQUE (TenantId, FacilityId, LabResultsId),
    CONSTRAINT UQ_LIS_ResultApproval_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_ResultApproval_Result FOREIGN KEY (TenantId, FacilityId, LabResultsId)
        REFERENCES dbo.LIS_LabResults(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_ResultApproval_Status FOREIGN KEY (TenantId, ApprovalStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_ResultApproval_ApprovedBy FOREIGN KEY (TenantId, ApprovedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   ResultHistory (audit trail)
   ======================================================================= */
CREATE TABLE dbo.LIS_ResultHistory (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_ResultHistory_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_ResultHistory_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ResultHistory_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ResultHistory_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ResultHistory_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ResultHistory_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabResultsId BIGINT NOT NULL,

    SnapshotResultNumeric DECIMAL(18,4) NULL,
    SnapshotResultText NVARCHAR(MAX) NULL,
    SnapshotIsAbnormal BIT NOT NULL CONSTRAINT DF_LIS_ResultHistory_SnapAbn DEFAULT (0),
    SnapshotAbnormalFlagReferenceValueId BIGINT NULL,
    SnapshotResultStatusReferenceValueId BIGINT NOT NULL,

    ChangedByDoctorId BIGINT NULL,
    ChangeReason NVARCHAR(1000) NULL,

    CONSTRAINT PK_LIS_ResultHistory PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_ResultHistory_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_ResultHistory_Result FOREIGN KEY (TenantId, FacilityId, LabResultsId)
        REFERENCES dbo.LIS_LabResults(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_ResultHistory_AbnormalFlag FOREIGN KEY (TenantId, SnapshotAbnormalFlagReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_ResultHistory_Status FOREIGN KEY (TenantId, SnapshotResultStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_ResultHistory_ChangedByDoctor FOREIGN KEY (TenantId, ChangedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   ReportHeader & ReportDetails
   ======================================================================= */
CREATE TABLE dbo.LIS_ReportHeader (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_ReportHeader_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_ReportHeader_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ReportHeader_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ReportHeader_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ReportHeader_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ReportHeader_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderId BIGINT NOT NULL,
    ReportNo NVARCHAR(60) NOT NULL,

    ReportTypeReferenceValueId BIGINT NULL,
    ReportStatusReferenceValueId BIGINT NOT NULL,

    PreparedOn DATETIME2(7) NULL,
    ReviewedOn DATETIME2(7) NULL,
    IssuedOn DATETIME2(7) NULL,

    PreparedByDoctorId BIGINT NULL,
    ReviewedByDoctorId BIGINT NULL,
    IssuedByDoctorId BIGINT NULL,

    CONSTRAINT PK_LIS_ReportHeader PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_ReportHeader_TenantFacility_OrderNo UNIQUE (TenantId, FacilityId, LabOrderId, ReportNo),
    CONSTRAINT UQ_LIS_ReportHeader_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_ReportHeader_Order FOREIGN KEY (TenantId, FacilityId, LabOrderId)
        REFERENCES dbo.LIS_LabOrder(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_ReportHeader_Type FOREIGN KEY (TenantId, ReportTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_ReportHeader_Status FOREIGN KEY (TenantId, ReportStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_ReportHeader_PreparedBy FOREIGN KEY (TenantId, PreparedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_LIS_ReportHeader_ReviewedBy FOREIGN KEY (TenantId, ReviewedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_LIS_ReportHeader_IssuedBy FOREIGN KEY (TenantId, IssuedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_ReportDetails (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_ReportDetails_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_ReportDetails_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ReportDetails_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ReportDetails_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ReportDetails_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ReportDetails_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReportHeaderId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_LIS_ReportDetails_Line DEFAULT (0),

    TestMasterId BIGINT NULL,
    TestParameterId BIGINT NULL,

    ResultDisplayText NVARCHAR(MAX) NULL,
    ReferenceRangeDisplayText NVARCHAR(MAX) NULL,
    LineNotes NVARCHAR(MAX) NULL,

    CONSTRAINT PK_LIS_ReportDetails PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_ReportDetails_TenantFacility_HeaderLine UNIQUE (TenantId, FacilityId, ReportHeaderId, LineNum),
    CONSTRAINT UQ_LIS_ReportDetails_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_ReportDetails_Header FOREIGN KEY (TenantId, FacilityId, ReportHeaderId)
        REFERENCES dbo.LIS_ReportHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_ReportDetails_TestMaster FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id),
    CONSTRAINT FK_LIS_ReportDetails_TestParameter FOREIGN KEY (TenantId, TestParameterId)
        REFERENCES dbo.LIS_TestParameters(TenantId, Id)
);
GO

/* =======================================================================
   Indexing (LIS hot paths)
   ======================================================================= */
CREATE NONCLUSTERED INDEX IX_LIS_LabOrder_TenantFacility_StatusOrdered
ON dbo.LIS_LabOrder (TenantId, FacilityId, OrderStatusReferenceValueId, OrderedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LIS_LabOrderItems_TenantFacility_OrderStatus
ON dbo.LIS_LabOrderItems (TenantId, FacilityId, LabOrderId, OrderItemStatusReferenceValueId, RequestedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LIS_SampleTracking_TenantFacility_TrackingNo
ON dbo.LIS_SampleTracking (TenantId, FacilityId, TrackingNo)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LIS_LabResults_TenantFacility_OrderItem_Param
ON dbo.LIS_LabResults (TenantId, FacilityId, LabOrderItemId, TestParameterId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LIS_ResultApproval_TenantFacility_ApprovedOn
ON dbo.LIS_ResultApproval (TenantId, FacilityId, ApprovedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LIS_ResultHistory_TenantFacility_ResultCreated
ON dbo.LIS_ResultHistory (TenantId, FacilityId, LabResultsId, CreatedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LIS_ReportHeader_TenantFacility_StatusIssued
ON dbo.LIS_ReportHeader (TenantId, FacilityId, ReportStatusReferenceValueId, IssuedOn DESC)
WHERE IsDeleted = 0;
GO

/* =======================================================================
   Cross-module FK: LabOrder -> Billing
   HMS_BillingItems has LabOrderId column; add FK constraint here.
   ======================================================================= */
IF COL_LENGTH('dbo.HMS_BillingItems', 'LabOrderId') IS NOT NULL
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.HMS_BillingItems WITH CHECK ADD CONSTRAINT FK_HMS_BillingItems_LabOrder
            FOREIGN KEY (TenantId, FacilityId, LabOrderId)
            REFERENCES dbo.LIS_LabOrder(TenantId, FacilityId, Id);
    END TRY
    BEGIN CATCH
        -- Ignore if already exists to allow re-runs.
    END CATCH
END;
GO

/* =======================================================================
   Suggested Partitioning (optional, for high write tables)
   - Partition by month using:
       * LIS_LabResults.MeasuredOn (or CreatedOn)
       * LIS_ResultHistory.CreatedOn
       * LIS_SampleTracking.TrackedOn
       * LIS_OrderStatusHistory.StatusOn
       * LIS_ReportDetails.CreatedOn
   ======================================================================= */

