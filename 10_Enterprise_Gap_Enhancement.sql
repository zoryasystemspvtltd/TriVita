SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  TriVita — Enterprise gap enhancement (script 10)
  Rules: CREATE only; no ALTER/DROP of existing objects.
  Depends: 00, 01 (Patient, Doctor, ReferenceDataValue, Facility, Department), 02_HMS, 04_LMS, 05_Pharmacy where FKs apply.
  Note: For split databases, remove cross-schema FKs and enforce in application layer.
*/

/* ======================================================================= HMS — Patient Master / UPID
   ======================================================================= */
CREATE TABLE dbo.HMS_PatientMaster (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PM_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PM_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PM_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PM_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PM_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PM_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    UPID NVARCHAR(40) NOT NULL,
    SharedPatientId BIGINT NULL,
    FullName NVARCHAR(250) NOT NULL,
    DateOfBirth DATE NULL,
    GenderReferenceValueId BIGINT NULL,
    PrimaryPhone NVARCHAR(40) NULL,
    PrimaryEmail NVARCHAR(200) NULL,

    CONSTRAINT PK_HMS_PatientMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_PM_Tenant_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_HMS_PM_Tenant_UPID UNIQUE (TenantId, UPID),

    CONSTRAINT FK_HMS_PM_Patient FOREIGN KEY (TenantId, SharedPatientId)
        REFERENCES dbo.Patient (TenantId, Id),
    CONSTRAINT FK_HMS_PM_Gender FOREIGN KEY (TenantId, GenderReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_PatientFacilityLink (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PFL_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PFL_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PFL_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PFL_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PFL_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PFL_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PatientMasterId BIGINT NOT NULL,
    LinkedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PFL_LinkedOn DEFAULT (SYSUTCDATETIME()),
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_HMS_PatientFacilityLink PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_PFL_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_PFL_Master_Facility UNIQUE (TenantId, PatientMasterId, FacilityId),

    CONSTRAINT FK_HMS_PFL_Master FOREIGN KEY (TenantId, PatientMasterId)
        REFERENCES dbo.HMS_PatientMaster (TenantId, Id),
    CONSTRAINT FK_HMS_PFL_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id)
);
GO

/* ======================================================================= HMS — IPD / Bed
   ======================================================================= */
CREATE TABLE dbo.HMS_Ward (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Ward_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Ward_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Ward_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Ward_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Ward_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Ward_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    WardCode NVARCHAR(40) NOT NULL,
    WardName NVARCHAR(200) NOT NULL,
    WardCategoryReferenceValueId BIGINT NULL,

    CONSTRAINT PK_HMS_Ward PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Ward_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_Ward_Tenant_Fac_Code UNIQUE (TenantId, FacilityId, WardCode),

    CONSTRAINT FK_HMS_Ward_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_HMS_Ward_Category FOREIGN KEY (TenantId, WardCategoryReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_Bed (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Bed_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Bed_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Bed_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Bed_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Bed_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Bed_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    WardId BIGINT NOT NULL,
    BedCode NVARCHAR(40) NOT NULL,
    BedCategoryReferenceValueId BIGINT NULL,
    BedOperationalStatusReferenceValueId BIGINT NOT NULL,
    CurrentAdmissionId BIGINT NULL,

    CONSTRAINT PK_HMS_Bed PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Bed_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_Bed_Tenant_Fac_Ward_Code UNIQUE (TenantId, FacilityId, WardId, BedCode),

    CONSTRAINT FK_HMS_Bed_Ward FOREIGN KEY (TenantId, FacilityId, WardId)
        REFERENCES dbo.HMS_Ward (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Bed_Category FOREIGN KEY (TenantId, BedCategoryReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id),
    CONSTRAINT FK_HMS_Bed_OpStatus FOREIGN KEY (TenantId, BedOperationalStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_Admission (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Adm_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Adm_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Adm_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Adm_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Adm_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Adm_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AdmissionNo NVARCHAR(60) NOT NULL,
    PatientMasterId BIGINT NOT NULL,
    BedId BIGINT NOT NULL,
    AdmissionStatusReferenceValueId BIGINT NOT NULL,
    AdmittedOn DATETIME2(7) NOT NULL,
    DischargedOn DATETIME2(7) NULL,
    AttendingDoctorId BIGINT NULL,

    CONSTRAINT PK_HMS_Admission PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Adm_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_Adm_Tenant_Fac_No UNIQUE (TenantId, FacilityId, AdmissionNo),

    CONSTRAINT FK_HMS_Adm_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_HMS_Adm_Master FOREIGN KEY (TenantId, PatientMasterId)
        REFERENCES dbo.HMS_PatientMaster (TenantId, Id),
    CONSTRAINT FK_HMS_Adm_Bed FOREIGN KEY (TenantId, FacilityId, BedId)
        REFERENCES dbo.HMS_Bed (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Adm_Status FOREIGN KEY (TenantId, AdmissionStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id),
    CONSTRAINT FK_HMS_Adm_Doctor FOREIGN KEY (TenantId, AttendingDoctorId)
        REFERENCES dbo.Doctor (TenantId, Id)
);
GO

ALTER TABLE dbo.HMS_Bed ADD CONSTRAINT FK_HMS_Bed_CurrentAdmission
    FOREIGN KEY (TenantId, FacilityId, CurrentAdmissionId)
    REFERENCES dbo.HMS_Admission (TenantId, FacilityId, Id);
GO

CREATE TABLE dbo.HMS_AdmissionTransfer (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_AdmTr_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_AdmTr_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_AdmTr_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_AdmTr_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_AdmTr_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_AdmTr_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AdmissionId BIGINT NOT NULL,
    FromBedId BIGINT NOT NULL,
    ToBedId BIGINT NOT NULL,
    TransferredOn DATETIME2(7) NOT NULL,
    Reason NVARCHAR(500) NULL,

    CONSTRAINT PK_HMS_AdmissionTransfer PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_AdmTr_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_AdmTr_Admission FOREIGN KEY (TenantId, FacilityId, AdmissionId)
        REFERENCES dbo.HMS_Admission (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_AdmTr_FromBed FOREIGN KEY (TenantId, FacilityId, FromBedId)
        REFERENCES dbo.HMS_Bed (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_AdmTr_ToBed FOREIGN KEY (TenantId, FacilityId, ToBedId)
        REFERENCES dbo.HMS_Bed (TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.HMS_HousekeepingStatus (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_HK_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_HK_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_HK_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_HK_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_HK_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_HK_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BedId BIGINT NOT NULL,
    HousekeepingStatusReferenceValueId BIGINT NOT NULL,
    RecordedOn DATETIME2(7) NOT NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_HMS_HousekeepingStatus PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_HK_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_HK_Bed FOREIGN KEY (TenantId, FacilityId, BedId)
        REFERENCES dbo.HMS_Bed (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_HK_Status FOREIGN KEY (TenantId, HousekeepingStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

/* ======================================================================= HMS — OT
   ======================================================================= */
CREATE TABLE dbo.HMS_OperationTheatre (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_OTRoom_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_OTRoom_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_OTRoom_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_OTRoom_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_OTRoom_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_OTRoom_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TheatreCode NVARCHAR(40) NOT NULL,
    TheatreName NVARCHAR(200) NOT NULL,
    DepartmentId BIGINT NULL,

    CONSTRAINT PK_HMS_OperationTheatre PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_OTRoom_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_OTRoom_Code UNIQUE (TenantId, FacilityId, TheatreCode),

    CONSTRAINT FK_HMS_OTRoom_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_HMS_OTRoom_Dept FOREIGN KEY (TenantId, DepartmentId)
        REFERENCES dbo.Department (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_SurgerySchedule (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Surg_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Surg_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Surg_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Surg_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Surg_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Surg_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    OperationTheatreId BIGINT NOT NULL,
    PatientMasterId BIGINT NOT NULL,
    SurgeonDoctorId BIGINT NOT NULL,
    ScheduledStartOn DATETIME2(7) NOT NULL,
    ScheduledEndOn DATETIME2(7) NULL,
    ProcedureSummary NVARCHAR(500) NULL,
    ScheduleStatusReferenceValueId BIGINT NOT NULL,

    CONSTRAINT PK_HMS_SurgerySchedule PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Surg_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_Surg_Theatre FOREIGN KEY (TenantId, FacilityId, OperationTheatreId)
        REFERENCES dbo.HMS_OperationTheatre (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Surg_Patient FOREIGN KEY (TenantId, PatientMasterId)
        REFERENCES dbo.HMS_PatientMaster (TenantId, Id),
    CONSTRAINT FK_HMS_Surg_Surgeon FOREIGN KEY (TenantId, SurgeonDoctorId)
        REFERENCES dbo.Doctor (TenantId, Id),
    CONSTRAINT FK_HMS_Surg_Status FOREIGN KEY (TenantId, ScheduleStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_AnesthesiaRecord (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Anes_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Anes_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Anes_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Anes_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Anes_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Anes_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SurgeryScheduleId BIGINT NOT NULL,
    AnesthesiologistDoctorId BIGINT NULL,
    RecordJson NVARCHAR(MAX) NULL,
    RecordedOn DATETIME2(7) NOT NULL,

    CONSTRAINT PK_HMS_AnesthesiaRecord PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Anes_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_Anes_Surgery FOREIGN KEY (TenantId, FacilityId, SurgeryScheduleId)
        REFERENCES dbo.HMS_SurgerySchedule (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Anes_Doctor FOREIGN KEY (TenantId, AnesthesiologistDoctorId)
        REFERENCES dbo.Doctor (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_PostOpRecord (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PostOp_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PostOp_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PostOp_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PostOp_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PostOp_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PostOp_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SurgeryScheduleId BIGINT NOT NULL,
    RecoveryNotes NVARCHAR(MAX) NULL,
    RecordedOn DATETIME2(7) NOT NULL,

    CONSTRAINT PK_HMS_PostOpRecord PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_PostOp_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_PostOp_Surgery FOREIGN KEY (TenantId, FacilityId, SurgeryScheduleId)
        REFERENCES dbo.HMS_SurgerySchedule (TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.HMS_OTConsumable (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_OTCon_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_OTCon_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_OTCon_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_OTCon_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_OTCon_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_OTCon_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SurgeryScheduleId BIGINT NOT NULL,
    ItemCode NVARCHAR(80) NOT NULL,
    ItemName NVARCHAR(250) NULL,
    Quantity DECIMAL(18,4) NOT NULL,
    UnitPrice DECIMAL(18,4) NULL,
    Billable BIT NOT NULL CONSTRAINT DF_HMS_OTCon_Bill DEFAULT (1),

    CONSTRAINT PK_HMS_OTConsumable PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_OTCon_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_OTCon_Surgery FOREIGN KEY (TenantId, FacilityId, SurgeryScheduleId)
        REFERENCES dbo.HMS_SurgerySchedule (TenantId, FacilityId, Id)
);
GO

/* ======================================================================= HMS — Billing extensions
   ======================================================================= */
CREATE TABLE dbo.HMS_PricingRule (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PR_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PR_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PR_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PR_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PR_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PR_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    RuleCode NVARCHAR(80) NOT NULL,
    RuleName NVARCHAR(250) NOT NULL,
    TariffTypeReferenceValueId BIGINT NULL,
    ServiceCode NVARCHAR(80) NOT NULL,
    UnitPrice DECIMAL(18,4) NOT NULL,
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_HMS_PricingRule PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_PR_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_PR_Tenant_Fac_Code UNIQUE (TenantId, FacilityId, RuleCode),

    CONSTRAINT FK_HMS_PR_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_HMS_PR_Tariff FOREIGN KEY (TenantId, TariffTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_PackageDefinition (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Pkg_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Pkg_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Pkg_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Pkg_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Pkg_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Pkg_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PackageCode NVARCHAR(80) NOT NULL,
    PackageName NVARCHAR(250) NOT NULL,
    BundlePrice DECIMAL(18,4) NOT NULL,
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_HMS_PackageDefinition PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Pkg_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_Pkg_Code UNIQUE (TenantId, FacilityId, PackageCode),

    CONSTRAINT FK_HMS_Pkg_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_PackageDefinitionLine (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PkgL_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PkgL_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PkgL_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PkgL_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PkgL_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PkgL_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PackageDefinitionId BIGINT NOT NULL,
    LineNumber INT NOT NULL,
    ServiceCode NVARCHAR(80) NOT NULL,
    Quantity DECIMAL(18,4) NOT NULL CONSTRAINT DF_HMS_PkgL_Qty DEFAULT (1),

    CONSTRAINT PK_HMS_PackageDefinitionLine PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_PkgL_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_PkgL_Pkg_Line UNIQUE (TenantId, FacilityId, PackageDefinitionId, LineNo),

    CONSTRAINT FK_HMS_PkgL_Header FOREIGN KEY (TenantId, FacilityId, PackageDefinitionId)
        REFERENCES dbo.HMS_PackageDefinition (TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.HMS_ProformaInvoice (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Proforma_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Proforma_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Proforma_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Proforma_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Proforma_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Proforma_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ProformaNo NVARCHAR(60) NOT NULL,
    PatientMasterId BIGINT NULL,
    VisitId BIGINT NULL,
    GrandTotal DECIMAL(18,4) NOT NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    LinesJson NVARCHAR(MAX) NULL,

    CONSTRAINT PK_HMS_ProformaInvoice PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Proforma_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_Proforma_No UNIQUE (TenantId, FacilityId, ProformaNo),

    CONSTRAINT FK_HMS_Proforma_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_HMS_Proforma_Master FOREIGN KEY (TenantId, PatientMasterId)
        REFERENCES dbo.HMS_PatientMaster (TenantId, Id),
    CONSTRAINT FK_HMS_Proforma_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Proforma_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

/* ======================================================================= HMS — Insurance / TPA
   ======================================================================= */
CREATE TABLE dbo.HMS_InsuranceProvider (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_InsProv_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_InsProv_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_InsProv_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_InsProv_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_InsProv_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_InsProv_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ProviderCode NVARCHAR(80) NOT NULL,
    ProviderName NVARCHAR(250) NOT NULL,
    TpaCategoryReferenceValueId BIGINT NULL,

    CONSTRAINT PK_HMS_InsuranceProvider PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_InsProv_Tenant_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_HMS_InsProv_Code UNIQUE (TenantId, ProviderCode),

    CONSTRAINT FK_HMS_InsProv_Cat FOREIGN KEY (TenantId, TpaCategoryReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_PreAuthorization (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PreAuth_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PreAuth_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PreAuth_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PreAuth_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PreAuth_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PreAuth_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PreAuthNo NVARCHAR(60) NOT NULL,
    InsuranceProviderId BIGINT NOT NULL,
    PatientMasterId BIGINT NOT NULL,
    RequestedOn DATETIME2(7) NOT NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    ApprovedAmount DECIMAL(18,4) NULL,
    Notes NVARCHAR(2000) NULL,

    CONSTRAINT PK_HMS_PreAuthorization PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_PreAuth_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_PreAuth_No UNIQUE (TenantId, FacilityId, PreAuthNo),

    CONSTRAINT FK_HMS_PreAuth_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_HMS_PreAuth_Provider FOREIGN KEY (TenantId, InsuranceProviderId)
        REFERENCES dbo.HMS_InsuranceProvider (TenantId, Id),
    CONSTRAINT FK_HMS_PreAuth_Master FOREIGN KEY (TenantId, PatientMasterId)
        REFERENCES dbo.HMS_PatientMaster (TenantId, Id),
    CONSTRAINT FK_HMS_PreAuth_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_Claim (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Claim_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Claim_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Claim_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Claim_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Claim_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Claim_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ClaimNo NVARCHAR(60) NOT NULL,
    InsuranceProviderId BIGINT NOT NULL,
    PatientMasterId BIGINT NOT NULL,
    BillingHeaderId BIGINT NULL,
    SubmittedOn DATETIME2(7) NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    ClaimAmount DECIMAL(18,4) NULL,

    CONSTRAINT PK_HMS_Claim PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Claim_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_HMS_Claim_No UNIQUE (TenantId, FacilityId, ClaimNo),

    CONSTRAINT FK_HMS_Claim_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_HMS_Claim_Provider FOREIGN KEY (TenantId, InsuranceProviderId)
        REFERENCES dbo.HMS_InsuranceProvider (TenantId, Id),
    CONSTRAINT FK_HMS_Claim_Master FOREIGN KEY (TenantId, PatientMasterId)
        REFERENCES dbo.HMS_PatientMaster (TenantId, Id),
    CONSTRAINT FK_HMS_Claim_Bill FOREIGN KEY (TenantId, FacilityId, BillingHeaderId)
        REFERENCES dbo.HMS_BillingHeader (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Claim_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

/* ======================================================================= HMS — Nursing
   ======================================================================= */
CREATE TABLE dbo.HMS_EmarEntry (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Emar_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Emar_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Emar_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Emar_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Emar_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Emar_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AdmissionId BIGINT NOT NULL,
    MedicationCode NVARCHAR(80) NOT NULL,
    ScheduledOn DATETIME2(7) NOT NULL,
    AdministeredOn DATETIME2(7) NULL,
    AdministrationStatusReferenceValueId BIGINT NOT NULL,
    NurseUserId BIGINT NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_HMS_EmarEntry PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Emar_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_Emar_Admission FOREIGN KEY (TenantId, FacilityId, AdmissionId)
        REFERENCES dbo.HMS_Admission (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Emar_Status FOREIGN KEY (TenantId, AdministrationStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.HMS_DoctorOrderAlert (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_DoAlert_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_DoAlert_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_DoAlert_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_DoAlert_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_DoAlert_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_DoAlert_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    VisitId BIGINT NULL,
    AdmissionId BIGINT NULL,
    DoctorId BIGINT NOT NULL,
    AlertTypeReferenceValueId BIGINT NOT NULL,
    Message NVARCHAR(1000) NOT NULL,
    AcknowledgedOn DATETIME2(7) NULL,

    CONSTRAINT PK_HMS_DoctorOrderAlert PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_DoAlert_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_DoAlert_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_HMS_DoAlert_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_DoAlert_Admission FOREIGN KEY (TenantId, FacilityId, AdmissionId)
        REFERENCES dbo.HMS_Admission (TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_DoAlert_Doctor FOREIGN KEY (TenantId, DoctorId)
        REFERENCES dbo.Doctor (TenantId, Id),
    CONSTRAINT FK_HMS_DoAlert_Type FOREIGN KEY (TenantId, AlertTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

/* ======================================================================= LMS — Logistics & report workflow
   ======================================================================= */
CREATE TABLE dbo.LMS_CollectionRequest (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ColReq_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ColReq_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ColReq_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ColReq_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ColReq_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ColReq_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    RequestNo NVARCHAR(60) NOT NULL,
    PatientId BIGINT NOT NULL,
    CollectionAddressJson NVARCHAR(MAX) NULL,
    RequestedWindowStart DATETIME2(7) NULL,
    RequestedWindowEnd DATETIME2(7) NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    ColdChainRequired BIT NOT NULL CONSTRAINT DF_LMS_ColReq_Cold DEFAULT (0),

    CONSTRAINT PK_LMS_CollectionRequest PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ColReq_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_LMS_ColReq_No UNIQUE (TenantId, FacilityId, RequestNo),

    CONSTRAINT FK_LMS_ColReq_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_LMS_ColReq_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient (TenantId, Id),
    CONSTRAINT FK_LMS_ColReq_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_RiderTracking (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_Rider_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_Rider_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_Rider_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_Rider_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_Rider_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_Rider_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CollectionRequestId BIGINT NOT NULL,
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    RecordedOn DATETIME2(7) NOT NULL,

    CONSTRAINT PK_LMS_RiderTracking PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_Rider_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_Rider_Request FOREIGN KEY (TenantId, FacilityId, CollectionRequestId)
        REFERENCES dbo.LMS_CollectionRequest (TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_SampleTransport (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ST_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ST_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ST_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ST_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ST_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ST_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CollectionRequestId BIGINT NOT NULL,
    TemperatureCelsius DECIMAL(5,2) NULL,
    RecordedOn DATETIME2(7) NOT NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_SampleTransport PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ST_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_ST_Request FOREIGN KEY (TenantId, FacilityId, CollectionRequestId)
        REFERENCES dbo.LMS_CollectionRequest (TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_ReportValidationStep (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_RVS_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_RVS_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_RVS_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_RVS_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_RVS_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_RVS_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderId BIGINT NOT NULL,
    ValidationLevel INT NOT NULL,
    ValidatorUserId BIGINT NOT NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    ValidatedOn DATETIME2(7) NULL,
    Comments NVARCHAR(1000) NULL,

    CONSTRAINT PK_LMS_ReportValidationStep PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_RVS_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_RVS_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_LMS_RVS_Order FOREIGN KEY (TenantId, FacilityId, LabOrderId)
        REFERENCES dbo.LIS_LabOrder (TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_RVS_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_ResultDeltaCheck (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_RDC_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_RDC_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_RDC_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_RDC_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_RDC_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_RDC_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CurrentLabResultId BIGINT NOT NULL,
    PriorLabResultId BIGINT NOT NULL,
    DeltaPercent DECIMAL(18,6) NULL,
    Flagged BIT NOT NULL CONSTRAINT DF_LMS_RDC_Flag DEFAULT (0),
    EvaluatedOn DATETIME2(7) NOT NULL,

    CONSTRAINT PK_LMS_ResultDeltaCheck PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_RDC_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_RDC_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_LMS_RDC_Current FOREIGN KEY (TenantId, FacilityId, CurrentLabResultId)
        REFERENCES dbo.LIS_LabResults (TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_RDC_Prior FOREIGN KEY (TenantId, FacilityId, PriorLabResultId)
        REFERENCES dbo.LIS_LabResults (TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_ReportDigitalSign (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_RDS_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_RDS_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_RDS_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_RDS_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_RDS_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_RDS_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReportHeaderId BIGINT NOT NULL,
    SignerUserId BIGINT NOT NULL,
    SignedOn DATETIME2(7) NOT NULL,
    SignatureReference NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_ReportDigitalSign PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_RDS_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_RDS_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_LMS_RDS_Report FOREIGN KEY (TenantId, FacilityId, ReportHeaderId)
        REFERENCES dbo.LIS_ReportHeader (TenantId, FacilityId, Id)
);
GO

/* ======================================================================= Pharmacy — tier / returns / controlled
   ======================================================================= */
CREATE TABLE dbo.Pharmacy_InventoryLocation (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PHR_Loc_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PHR_Loc_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_Loc_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PHR_Loc_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_Loc_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PHR_Loc_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LocationCode NVARCHAR(80) NOT NULL,
    LocationName NVARCHAR(250) NOT NULL,
    LocationTypeReferenceValueId BIGINT NOT NULL,
    ParentLocationId BIGINT NULL,

    CONSTRAINT PK_Pharmacy_InventoryLocation PRIMARY KEY (Id),
    CONSTRAINT UQ_PHR_Loc_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_PHR_Loc_Code UNIQUE (TenantId, FacilityId, LocationCode),

    CONSTRAINT FK_PHR_Loc_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_PHR_Loc_Type FOREIGN KEY (TenantId, LocationTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id),
    CONSTRAINT FK_PHR_Loc_Parent FOREIGN KEY (TenantId, FacilityId, ParentLocationId)
        REFERENCES dbo.Pharmacy_InventoryLocation (TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.Pharmacy_SalesReturn (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PHR_SR_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PHR_SR_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_SR_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PHR_SR_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_SR_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PHR_SR_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReturnNo NVARCHAR(60) NOT NULL,
    OriginalSalesId BIGINT NOT NULL,
    ReturnReasonReferenceValueId BIGINT NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    ReturnedOn DATETIME2(7) NOT NULL,

    CONSTRAINT PK_Pharmacy_SalesReturn PRIMARY KEY (Id),
    CONSTRAINT UQ_PHR_SR_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_PHR_SR_No UNIQUE (TenantId, FacilityId, ReturnNo),

    CONSTRAINT FK_PHR_SR_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_PHR_SR_Sale FOREIGN KEY (TenantId, FacilityId, OriginalSalesId)
        REFERENCES dbo.PharmacySales (TenantId, FacilityId, Id),
    CONSTRAINT FK_PHR_SR_Reason FOREIGN KEY (TenantId, ReturnReasonReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id),
    CONSTRAINT FK_PHR_SR_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.Pharmacy_SalesReturnItem (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PHR_SRI_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PHR_SRI_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_SRI_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PHR_SRI_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_SRI_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PHR_SRI_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SalesReturnId BIGINT NOT NULL,
    OriginalSalesItemId BIGINT NOT NULL,
    QuantityReturned DECIMAL(18,4) NOT NULL,
    ReconciliationStatusReferenceValueId BIGINT NULL,

    CONSTRAINT PK_Pharmacy_SalesReturnItem PRIMARY KEY (Id),
    CONSTRAINT UQ_PHR_SRI_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_PHR_SRI_Return FOREIGN KEY (TenantId, FacilityId, SalesReturnId)
        REFERENCES dbo.Pharmacy_SalesReturn (TenantId, FacilityId, Id),
    CONSTRAINT FK_PHR_SRI_OrigLine FOREIGN KEY (TenantId, FacilityId, OriginalSalesItemId)
        REFERENCES dbo.PharmacySalesItems (TenantId, FacilityId, Id),
    CONSTRAINT FK_PHR_SRI_Recon FOREIGN KEY (TenantId, ReconciliationStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue (TenantId, Id)
);
GO

CREATE TABLE dbo.Pharmacy_ControlledDrugRegister (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PHR_CDR_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PHR_CDR_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_CDR_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PHR_CDR_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_CDR_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PHR_CDR_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PharmacySalesItemId BIGINT NOT NULL,
    PrescribingDoctorId BIGINT NOT NULL,
    PatientId BIGINT NOT NULL,
    PatientAcknowledged BIT NOT NULL CONSTRAINT DF_PHR_CDR_Ack DEFAULT (0),
    PatientAcknowledgedOn DATETIME2(7) NULL,
    RegisterEntryOn DATETIME2(7) NOT NULL,

    CONSTRAINT PK_Pharmacy_ControlledDrugRegister PRIMARY KEY (Id),
    CONSTRAINT UQ_PHR_CDR_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_PHR_CDR_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_PHR_CDR_Line FOREIGN KEY (TenantId, FacilityId, PharmacySalesItemId)
        REFERENCES dbo.PharmacySalesItems (TenantId, FacilityId, Id),
    CONSTRAINT FK_PHR_CDR_Doctor FOREIGN KEY (TenantId, PrescribingDoctorId)
        REFERENCES dbo.Doctor (TenantId, Id),
    CONSTRAINT FK_PHR_CDR_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient (TenantId, Id)
);
GO

CREATE TABLE dbo.Pharmacy_BatchStockLocation (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PHR_BSL_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PHR_BSL_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_BSL_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PHR_BSL_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_BSL_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PHR_BSL_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BatchStockId BIGINT NOT NULL,
    InventoryLocationId BIGINT NOT NULL,
    QuantityOnHand DECIMAL(18,4) NOT NULL,

    CONSTRAINT PK_Pharmacy_BatchStockLocation PRIMARY KEY (Id),
    CONSTRAINT UQ_PHR_BSL_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_PHR_BSL_Batch_Loc UNIQUE (TenantId, FacilityId, BatchStockId, InventoryLocationId),

    CONSTRAINT FK_PHR_BSL_Batch FOREIGN KEY (TenantId, FacilityId, BatchStockId)
        REFERENCES dbo.BatchStock (TenantId, FacilityId, Id),
    CONSTRAINT FK_PHR_BSL_Loc FOREIGN KEY (TenantId, FacilityId, InventoryLocationId)
        REFERENCES dbo.Pharmacy_InventoryLocation (TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.Pharmacy_ReorderPolicy (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PHR_RP_IsActive2 DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PHR_RP_IsDeleted2 DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_RP_CreatedOn2 DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PHR_RP_CreatedBy2 DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PHR_RP_ModifiedOn2 DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PHR_RP_ModifiedBy2 DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BatchStockId BIGINT NOT NULL,
    LeadTimeDays INT NOT NULL CONSTRAINT DF_PHR_RP_Lead2 DEFAULT (0),
    EconomicOrderQty DECIMAL(18,4) NULL,

    CONSTRAINT PK_Pharmacy_ReorderPolicy PRIMARY KEY (Id),
    CONSTRAINT UQ_PHR_RP_TenantFacility_Id2 UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_PHR_RP_Batch2 UNIQUE (TenantId, FacilityId, BatchStockId),

    CONSTRAINT FK_PHR_RP_Batch2 FOREIGN KEY (TenantId, FacilityId, BatchStockId)
        REFERENCES dbo.BatchStock (TenantId, FacilityId, Id)
);
GO
