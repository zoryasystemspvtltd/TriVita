SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  TriVita — LMS / LIS workflow enhancement (script 11)
  Rules: CREATE only; no ALTER/DROP of existing objects.
  Depends: 00 (Patient, Facility, ReferenceDataValue, Unit), 03_LIS (LIS_* where referenced), 04_LMS (Equipment),
           07 (LMS_TestPackage, etc.).

  Architecture:
    - LMS: master catalog, bookings, barcodes, equipment mappings (this script + application APIs).
    - LIS: analyzer middleware result storage only (LIS_AnalyzerResult*).

  Split databases: remove cross-schema FKs; enforce in application.
*/

/* ======================================================================= LMS — Equipment type & facility mapping
   ======================================================================= */
CREATE TABLE dbo.LMS_EquipmentType (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ET_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ET_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ET_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ET_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ET_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ET_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TypeCode NVARCHAR(80) NOT NULL,
    TypeName NVARCHAR(250) NOT NULL,
    Description NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_EquipmentType PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ET_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_LMS_ET_Tenant_TypeCode UNIQUE (TenantId, TypeCode)
);
GO

CREATE TABLE dbo.LMS_EquipmentFacilityMapping (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_EFM_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_EFM_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_EFM_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_EFM_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_EFM_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_EFM_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EquipmentFacilityId BIGINT NOT NULL,
    EquipmentId BIGINT NOT NULL,
    MappedFacilityId BIGINT NOT NULL,
    MappingNotes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_EquipmentFacilityMapping PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_EFM_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_EFM_UniqueMap UNIQUE (TenantId, EquipmentFacilityId, EquipmentId, MappedFacilityId),

    CONSTRAINT FK_LMS_EFM_Equipment FOREIGN KEY (TenantId, EquipmentFacilityId, EquipmentId)
        REFERENCES dbo.Equipment(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_EFM_MappedFacility FOREIGN KEY (TenantId, MappedFacilityId)
        REFERENCES dbo.Facility(TenantId, Id),
    CONSTRAINT FK_LMS_EFM_ScopeFacility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id)
);
GO

/* ======================================================================= LMS — Catalog test / parameter (LMS-owned master)
   ======================================================================= */
CREATE TABLE dbo.LMS_CatalogTest (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_CT_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_CT_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CT_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CT_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CT_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CT_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestCode NVARCHAR(80) NOT NULL,
    TestName NVARCHAR(250) NOT NULL,
    TestDescription NVARCHAR(1000) NULL,
    DisciplineReferenceValueId BIGINT NOT NULL,
    SampleTypeReferenceValueId BIGINT NULL,
    IsRadiology BIT NOT NULL CONSTRAINT DF_LMS_CT_IsRad DEFAULT (0),
    DefaultUnitId BIGINT NULL,

    CONSTRAINT PK_LMS_CatalogTest PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_CT_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_CT_TenantFac_Code UNIQUE (TenantId, FacilityId, TestCode),

    CONSTRAINT FK_LMS_CT_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id),
    CONSTRAINT FK_LMS_CT_Discipline FOREIGN KEY (TenantId, DisciplineReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_CT_SampleType FOREIGN KEY (TenantId, SampleTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_CT_Unit FOREIGN KEY (TenantId, DefaultUnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_CatalogParameter (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_CP_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_CP_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CP_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CP_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CP_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CP_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ParameterCode NVARCHAR(100) NOT NULL,
    ParameterName NVARCHAR(300) NOT NULL,
    IsNumeric BIT NOT NULL CONSTRAINT DF_LMS_CP_IsNum DEFAULT (1),
    UnitId BIGINT NULL,
    ParameterNotes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_CatalogParameter PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_CP_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_LMS_CP_Tenant_Fac_Code UNIQUE (TenantId, FacilityId, ParameterCode),

    CONSTRAINT FK_LMS_CP_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id),
    CONSTRAINT FK_LMS_CP_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_CatalogReferenceRange (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_CRR_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_CRR_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CRR_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CRR_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CRR_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CRR_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CatalogParameterId BIGINT NOT NULL,
    SexReferenceValueId BIGINT NULL,
    AgeFromYears INT NULL,
    AgeToYears INT NULL,
    MinValue DECIMAL(18,4) NULL,
    MaxValue DECIMAL(18,4) NULL,
    RangeText NVARCHAR(500) NULL,
    RangeNotes NVARCHAR(800) NULL,

    CONSTRAINT PK_LMS_CatalogReferenceRange PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_CRR_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_CRR_Parameter FOREIGN KEY (TenantId, CatalogParameterId)
        REFERENCES dbo.LMS_CatalogParameter(TenantId, Id),
    CONSTRAINT FK_LMS_CRR_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id),
    CONSTRAINT FK_LMS_CRR_Sex FOREIGN KEY (TenantId, SexReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_CatalogTestParameterMap (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_CTPM_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_CTPM_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CTPM_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CTPM_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CTPM_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CTPM_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CatalogTestId BIGINT NOT NULL,
    CatalogParameterId BIGINT NOT NULL,
    DisplayOrder INT NOT NULL CONSTRAINT DF_LMS_CTPM_Order DEFAULT (0),

    CONSTRAINT PK_LMS_CatalogTestParameterMap PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_CTPM_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_CTPM_Test_Param UNIQUE (TenantId, FacilityId, CatalogTestId, CatalogParameterId),

    CONSTRAINT FK_LMS_CTPM_Test FOREIGN KEY (TenantId, FacilityId, CatalogTestId)
        REFERENCES dbo.LMS_CatalogTest(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_CTPM_Param FOREIGN KEY (TenantId, CatalogParameterId)
        REFERENCES dbo.LMS_CatalogParameter(TenantId, Id),
    CONSTRAINT FK_LMS_CTPM_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_CatalogPackageParameterMap (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_CPPM_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_CPPM_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CPPM_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CPPM_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CPPM_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CPPM_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestPackageId BIGINT NOT NULL,
    CatalogParameterId BIGINT NOT NULL,
    CatalogTestId BIGINT NULL,

    CONSTRAINT PK_LMS_CatalogPackageParameterMap PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_CPPM_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_CPPM_Unique UNIQUE (TenantId, FacilityId, TestPackageId, CatalogParameterId, CatalogTestId),

    CONSTRAINT FK_LMS_CPPM_Package FOREIGN KEY (TenantId, FacilityId, TestPackageId)
        REFERENCES dbo.LMS_TestPackage(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_CPPM_Param FOREIGN KEY (TenantId, CatalogParameterId)
        REFERENCES dbo.LMS_CatalogParameter(TenantId, Id),
    CONSTRAINT FK_LMS_CPPM_Test FOREIGN KEY (TenantId, FacilityId, CatalogTestId)
        REFERENCES dbo.LMS_CatalogTest(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_CPPM_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id)
);
GO

/* ======================================================================= LMS — Equipment test codes & mappings
   ======================================================================= */
CREATE TABLE dbo.LMS_EquipmentTestMaster (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ETM_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ETM_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ETM_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ETM_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ETM_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ETM_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EquipmentId BIGINT NOT NULL,
    CatalogTestId BIGINT NOT NULL,
    EquipmentAssayCode NVARCHAR(120) NOT NULL,
    EquipmentAssayName NVARCHAR(300) NULL,

    CONSTRAINT PK_LMS_EquipmentTestMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ETM_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_ETM_Equip_Code UNIQUE (TenantId, FacilityId, EquipmentId, EquipmentAssayCode),

    CONSTRAINT FK_LMS_ETM_Equipment FOREIGN KEY (TenantId, FacilityId, EquipmentId)
        REFERENCES dbo.Equipment(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ETM_Test FOREIGN KEY (TenantId, FacilityId, CatalogTestId)
        REFERENCES dbo.LMS_CatalogTest(TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_CatalogTestEquipmentMap (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_CTEM_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_CTEM_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CTEM_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CTEM_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CTEM_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CTEM_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CatalogTestId BIGINT NOT NULL,
    EquipmentId BIGINT NOT NULL,
    IsPreferred BIT NOT NULL CONSTRAINT DF_LMS_CTEM_Pref DEFAULT (0),

    CONSTRAINT PK_LMS_CatalogTestEquipmentMap PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_CTEM_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_CTEM_Test_Equip UNIQUE (TenantId, FacilityId, CatalogTestId, EquipmentId),

    CONSTRAINT FK_LMS_CTEM_Test FOREIGN KEY (TenantId, FacilityId, CatalogTestId)
        REFERENCES dbo.LMS_CatalogTest(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_CTEM_Equipment FOREIGN KEY (TenantId, FacilityId, EquipmentId)
        REFERENCES dbo.Equipment(TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_CatalogPackageTestLineMap (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_CPTLM_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_CPTLM_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CPTLM_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CPTLM_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_CPTLM_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_CPTLM_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestPackageId BIGINT NOT NULL,
    LineNum INT NOT NULL,
    CatalogTestId BIGINT NOT NULL,

    CONSTRAINT PK_LMS_CatalogPackageTestLineMap PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_CPTLM_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_CPTLM_Pkg_Line UNIQUE (TenantId, FacilityId, TestPackageId, LineNum),

    CONSTRAINT FK_LMS_CPTLM_Package FOREIGN KEY (TenantId, FacilityId, TestPackageId)
        REFERENCES dbo.LMS_TestPackage(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_CPTLM_Test FOREIGN KEY (TenantId, FacilityId, CatalogTestId)
        REFERENCES dbo.LMS_CatalogTest(TenantId, FacilityId, Id)
);
GO

/* ======================================================================= LMS — Test booking & barcode (workflow)
   ======================================================================= */
CREATE TABLE dbo.LMS_LabTestBooking (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_LTB_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_LTB_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LTB_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LTB_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LTB_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LTB_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BookingNo NVARCHAR(60) NOT NULL,
    PatientId BIGINT NOT NULL,
    VisitId BIGINT NULL,
    SourceReferenceValueId BIGINT NULL,
    BookingNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_LMS_LabTestBooking PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_LTB_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_LTB_BookingNo UNIQUE (TenantId, FacilityId, BookingNo),

    CONSTRAINT FK_LMS_LTB_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id),
    CONSTRAINT FK_LMS_LTB_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_LMS_LTB_Source FOREIGN KEY (TenantId, SourceReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_LabTestBookingItem (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_LTBI_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_LTBI_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LTBI_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LTBI_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LTBI_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LTBI_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabTestBookingId BIGINT NOT NULL,
    CatalogTestId BIGINT NOT NULL,
    WorkflowStatusReferenceValueId BIGINT NOT NULL,
    LineNotes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_LabTestBookingItem PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_LTBI_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_LTBI_Booking FOREIGN KEY (TenantId, FacilityId, LabTestBookingId)
        REFERENCES dbo.LMS_LabTestBooking(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LTBI_Test FOREIGN KEY (TenantId, FacilityId, CatalogTestId)
        REFERENCES dbo.LMS_CatalogTest(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LTBI_Status FOREIGN KEY (TenantId, WorkflowStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_LabSampleBarcode (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_LSB_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_LSB_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LSB_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LSB_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LSB_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LSB_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BarcodeValue NVARCHAR(120) NOT NULL,
    TestBookingItemId BIGINT NOT NULL,
    SampleTypeReferenceValueId BIGINT NULL,
    BarcodeStatusReferenceValueId BIGINT NOT NULL,
    RegisteredFromSystem NVARCHAR(120) NULL,

    CONSTRAINT PK_LMS_LabSampleBarcode PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_LSB_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_LSB_Tenant_Barcode UNIQUE (TenantId, BarcodeValue),

    CONSTRAINT FK_LMS_LSB_Item FOREIGN KEY (TenantId, FacilityId, TestBookingItemId)
        REFERENCES dbo.LMS_LabTestBookingItem(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LSB_SampleType FOREIGN KEY (TenantId, SampleTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_LSB_Status FOREIGN KEY (TenantId, BarcodeStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_LSB_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LMS_LabSampleBarcode_Tenant_Value
    ON dbo.LMS_LabSampleBarcode (TenantId, BarcodeValue) WHERE IsDeleted = 0;
GO

/* ======================================================================= LIS — Analyzer middleware results (no catalog duplication)
   ======================================================================= */
CREATE TABLE dbo.LIS_AnalyzerResultHeader (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_ARH_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_ARH_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ARH_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ARH_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ARH_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ARH_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BarcodeValue NVARCHAR(120) NOT NULL,
    LmsTestBookingItemId BIGINT NOT NULL,
    LmsCatalogTestId BIGINT NOT NULL,
    EquipmentId BIGINT NULL,
    EquipmentAssayCode NVARCHAR(120) NULL,
    ReceivedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ARH_Received DEFAULT (SYSUTCDATETIME()),

    TechnicallyVerified BIT NOT NULL CONSTRAINT DF_LIS_ARH_TechVer DEFAULT (0),
    TechnicallyVerifiedOn DATETIME2(7) NULL,
    ReadyForDispatch BIT NOT NULL CONSTRAINT DF_LIS_ARH_RFD DEFAULT (0),
    ResultStatusReferenceValueId BIGINT NOT NULL,

    CONSTRAINT PK_LIS_AnalyzerResultHeader PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_ARH_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_ARH_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id),
    CONSTRAINT FK_LIS_ARH_ResultStatus FOREIGN KEY (TenantId, ResultStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_AnalyzerResultLine (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_ARL_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_ARL_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ARL_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ARL_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ARL_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ARL_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AnalyzerResultHeaderId BIGINT NOT NULL,
    LmsCatalogParameterId BIGINT NULL,
    EquipmentResultCode NVARCHAR(120) NULL,
    ResultNumeric DECIMAL(18,4) NULL,
    ResultText NVARCHAR(2000) NULL,
    ResultUnitId BIGINT NULL,
    LineStatusReferenceValueId BIGINT NOT NULL,

    CONSTRAINT PK_LIS_AnalyzerResultLine PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_ARL_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_ARL_Header FOREIGN KEY (TenantId, FacilityId, AnalyzerResultHeaderId)
        REFERENCES dbo.LIS_AnalyzerResultHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_ARL_Unit FOREIGN KEY (TenantId, ResultUnitId)
        REFERENCES dbo.Unit(TenantId, Id),
    CONSTRAINT FK_LIS_ARL_LineStatus FOREIGN KEY (TenantId, LineStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LIS_AnalyzerResultHeader_Tenant_Barcode
    ON dbo.LIS_AnalyzerResultHeader (TenantId, FacilityId, BarcodeValue) WHERE IsDeleted = 0;
GO
