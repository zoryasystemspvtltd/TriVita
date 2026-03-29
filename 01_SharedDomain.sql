SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  Shared Domain (Core Master Data)
  - Patients
  - PatientIdentifiers
  - Doctors
  - Units
  - ReferenceData (enum-like)

  Notes:
  - Uses root tables: dbo.Address, dbo.ContactDetails, dbo.Department
  - All tables include global audit/tenant standard.
  - FacilityId is nullable on shared masters (transactional tables will make it NOT NULL).
*/

/* =======================================================================
   Units (for lab/pharmacy)
   ======================================================================= */
CREATE TABLE dbo.Unit (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Unit_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Unit_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Unit_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Unit_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Unit_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Unit_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    UnitCode NVARCHAR(80) NOT NULL,
    UnitName NVARCHAR(200) NOT NULL,
    UnitType NVARCHAR(100) NULL,

    CONSTRAINT PK_Unit PRIMARY KEY (Id),
    CONSTRAINT UQ_Unit_Tenant_Code UNIQUE (TenantId, UnitCode),
    CONSTRAINT UQ_Unit_TenantId_Id UNIQUE (TenantId, Id)
);
GO
-- Purpose: Measurement/dispensing units (e.g., mg, mL, mmol/L).

/* =======================================================================
   ReferenceDataDefinition
   ======================================================================= */
CREATE TABLE dbo.ReferenceDataDefinition (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ReferenceDataDefinition_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_ReferenceDataDefinition_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ReferenceDataDefinition_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_ReferenceDataDefinition_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ReferenceDataDefinition_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_ReferenceDataDefinition_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    DefinitionCode NVARCHAR(150) NOT NULL,
    DefinitionName NVARCHAR(300) NOT NULL,
    Description NVARCHAR(500) NULL,

    CONSTRAINT PK_ReferenceDataDefinition PRIMARY KEY (Id),
    CONSTRAINT UQ_ReferenceDataDefinition_Tenant_Code UNIQUE (TenantId, DefinitionCode),
    CONSTRAINT UQ_ReferenceDataDefinition_TenantId_Id UNIQUE (TenantId, Id)
);
GO
-- Purpose: Groups configurable dropdown values (enums) per tenant.

/* =======================================================================
   ReferenceDataValue
   ======================================================================= */
CREATE TABLE dbo.ReferenceDataValue (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ReferenceDataValue_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_ReferenceDataValue_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ReferenceDataValue_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_ReferenceDataValue_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ReferenceDataValue_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_ReferenceDataValue_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReferenceDataDefinitionId BIGINT NOT NULL,
    ValueCode NVARCHAR(150) NOT NULL,
    ValueName NVARCHAR(300) NOT NULL,
    ValueText NVARCHAR(1000) NULL,
    SortOrder INT NOT NULL CONSTRAINT DF_ReferenceDataValue_SortOrder DEFAULT (0),

    CONSTRAINT PK_ReferenceDataValue PRIMARY KEY (Id),
    CONSTRAINT UQ_ReferenceDataValue_Tenant_Def_Value UNIQUE (TenantId, ReferenceDataDefinitionId, ValueCode),
    CONSTRAINT UQ_ReferenceDataValue_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_ReferenceDataValue_Def FOREIGN KEY (TenantId, ReferenceDataDefinitionId)
        REFERENCES dbo.ReferenceDataDefinition(TenantId, Id)
);
GO
-- Purpose: Individual enum/config values (status codes, categories, etc.).

/* =======================================================================
   Doctor (basic info)
   ======================================================================= */
CREATE TABLE dbo.Doctor (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Doctor_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Doctor_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Doctor_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Doctor_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Doctor_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Doctor_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    DoctorCode NVARCHAR(80) NULL,
    DoctorName NVARCHAR(250) NOT NULL,
    MedicalLicenseNumber NVARCHAR(120) NULL,

    DepartmentId BIGINT NULL,         -- optional: specialization / unit linkage
    SpecializationReferenceValueId BIGINT NULL, -- optional: from ReferenceDataValue

    PrimaryContactId BIGINT NULL,
    PrimaryAddressId BIGINT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_Doctor PRIMARY KEY (Id),
    CONSTRAINT UQ_Doctor_Tenant_License UNIQUE (TenantId, MedicalLicenseNumber),
    CONSTRAINT UQ_Doctor_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_Doctor_Department FOREIGN KEY (TenantId, DepartmentId)
        REFERENCES dbo.Department(TenantId, Id),
    CONSTRAINT FK_Doctor_Contact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails(TenantId, Id),
    CONSTRAINT FK_Doctor_Address FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address(TenantId, Id),
    CONSTRAINT FK_Doctor_Specialization FOREIGN KEY (TenantId, SpecializationReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO
-- Purpose: Provider master used across HMS/LIS/LMS/Pharmacy.

/* =======================================================================
   Patient
   ======================================================================= */
CREATE TABLE dbo.Patient (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Patient_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Patient_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Patient_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Patient_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Patient_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Patient_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    FirstName NVARCHAR(120) NOT NULL,
    MiddleName NVARCHAR(120) NULL,
    LastName NVARCHAR(120) NOT NULL,

    GenderReferenceValueId BIGINT NULL,
    BloodGroupReferenceValueId BIGINT NULL,
    MaritalStatusReferenceValueId BIGINT NULL,
    DateOfBirth DATE NULL,
    Nationality NVARCHAR(120) NULL,

    PrimaryAddressId BIGINT NULL,
    PrimaryContactId BIGINT NULL,

    DeceasedOn DATETIME2(7) NULL,
    DeceasedReason NVARCHAR(500) NULL,

    CONSTRAINT PK_Patient PRIMARY KEY (Id),
    CONSTRAINT UQ_Patient_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_Patient_Gender FOREIGN KEY (TenantId, GenderReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_Patient_BloodGroup FOREIGN KEY (TenantId, BloodGroupReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_Patient_MaritalStatus FOREIGN KEY (TenantId, MaritalStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_Patient_Address FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address(TenantId, Id),
    CONSTRAINT FK_Patient_Contact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails(TenantId, Id)
);
GO
-- Purpose: Demographics and contact pointers for clinical workflows.

/* =======================================================================
   PatientIdentifier (MRN/UHID/etc.)
   ======================================================================= */
CREATE TABLE dbo.PatientIdentifier (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_PatientIdentifier_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_PatientIdentifier_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PatientIdentifier_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_PatientIdentifier_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_PatientIdentifier_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_PatientIdentifier_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PatientId BIGINT NOT NULL,
    IdentifierTypeReferenceValueId BIGINT NOT NULL, -- e.g. MRN, UHID
    IdentifierValue NVARCHAR(120) NOT NULL,
    IssuingAuthority NVARCHAR(200) NULL,
    IsPrimary BIT NOT NULL CONSTRAINT DF_PatientIdentifier_IsPrimary DEFAULT (0),

    CONSTRAINT PK_PatientIdentifier PRIMARY KEY (Id),
    CONSTRAINT UQ_PatientIdentifier_Tenant_Type_Value UNIQUE (TenantId, IdentifierTypeReferenceValueId, IdentifierValue),
    CONSTRAINT UQ_PatientIdentifier_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_PatientIdentifier_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_PatientIdentifier_IdentifierType FOREIGN KEY (TenantId, IdentifierTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO
-- Purpose: MRN/UHID and other patient identifiers.

/* =======================================================================
   Indexing (shared domain)
   ======================================================================= */
CREATE NONCLUSTERED INDEX IX_Unit_Tenant_IsDeleted ON dbo.Unit (TenantId, IsDeleted)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_ReferenceDataDefinition_Tenant_Code ON dbo.ReferenceDataDefinition (TenantId, DefinitionCode)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_ReferenceDataValue_Tenant_Def ON dbo.ReferenceDataValue (TenantId, ReferenceDataDefinitionId, IsDeleted)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_Doctor_Tenant_Department ON dbo.Doctor (TenantId, DepartmentId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_Patient_Tenant_Name_DOB ON dbo.Patient (TenantId, LastName, FirstName, DateOfBirth)
INCLUDE (PrimaryContactId, PrimaryAddressId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_PatientIdentifier_Tenant_Type_Value ON dbo.PatientIdentifier (TenantId, IdentifierTypeReferenceValueId, IdentifierValue)
WHERE IsDeleted = 0;
GO

