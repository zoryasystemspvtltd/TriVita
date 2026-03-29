SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  Healthcare SaaS - Enterprise Hierarchy (Root Foundation)
  Entities:
    Enterprise -> Company -> BusinessUnit -> Facility -> Department

  Global standards enforced on every table:
    Id BIGINT IDENTITY PK
    TenantId BIGINT NOT NULL
    FacilityId BIGINT NULL (NOT NULL is used for facility-bound entities)
    IsActive BIT DEFAULT(1)
    IsDeleted BIT DEFAULT(0)
    CreatedOn DATETIME2(7), CreatedBy BIGINT
    ModifiedOn DATETIME2(7), ModifiedBy BIGINT
    RowVersion ROWVERSION
*/

/* =======================================================================
   Reusable Address
   ======================================================================= */
CREATE TABLE dbo.Address (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Address_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Address_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Address_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Address_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Address_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Address_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AddressType NVARCHAR(50) NULL,      -- Registered / Operational / Billing, etc.
    Line1 NVARCHAR(250) NOT NULL,
    Line2 NVARCHAR(250) NULL,
    Area NVARCHAR(150) NULL,
    City NVARCHAR(120) NULL,
    StateProvince NVARCHAR(120) NULL,
    PostalCode NVARCHAR(30) NULL,
    CountryCode NVARCHAR(10) NULL,
    Latitude DECIMAL(9,6) NULL,
    Longitude DECIMAL(9,6) NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_Address PRIMARY KEY (Id),
    CONSTRAINT UQ_Address_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_Address_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

/* =======================================================================
   Reusable ContactDetails
   ======================================================================= */
CREATE TABLE dbo.ContactDetails (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ContactDetails_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_ContactDetails_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ContactDetails_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_ContactDetails_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ContactDetails_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_ContactDetails_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ContactType NVARCHAR(50) NOT NULL,   -- Phone / Email / Fax / Website
    ContactValue NVARCHAR(300) NOT NULL,
    CountryCode NVARCHAR(20) NULL,
    Extension NVARCHAR(20) NULL,
    IsPrimary BIT NOT NULL CONSTRAINT DF_ContactDetails_IsPrimary DEFAULT (0),

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_ContactDetails PRIMARY KEY (Id),
    CONSTRAINT UQ_ContactDetails_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_ContactDetails_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

/* =======================================================================
   Enterprise
   ======================================================================= */
CREATE TABLE dbo.Enterprise (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Enterprise_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Enterprise_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Enterprise_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Enterprise_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Enterprise_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Enterprise_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EnterpriseCode NVARCHAR(80) NOT NULL,
    EnterpriseName NVARCHAR(250) NOT NULL,
    RegistrationDetails NVARCHAR(1000) NULL,
    GlobalSettingsJson NVARCHAR(MAX) NULL,

    PrimaryAddressId BIGINT NULL,
    PrimaryContactId BIGINT NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_Enterprise PRIMARY KEY (Id),
    CONSTRAINT UQ_Enterprise_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_Enterprise_Tenant_Code UNIQUE (TenantId, EnterpriseCode),
    CONSTRAINT CK_Enterprise_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_Enterprise_PrimaryAddress FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address (TenantId, Id),
    CONSTRAINT FK_Enterprise_PrimaryContact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails (TenantId, Id)
);
GO

/* =======================================================================
   Company (Legal Entity)
   ======================================================================= */
CREATE TABLE dbo.Company (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Company_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Company_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Company_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Company_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Company_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Company_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EnterpriseId BIGINT NOT NULL,
    CompanyCode NVARCHAR(80) NOT NULL,
    CompanyName NVARCHAR(250) NOT NULL,

    PAN NVARCHAR(30) NULL,
    GSTIN NVARCHAR(30) NULL,
    LegalIdentifier1 NVARCHAR(100) NULL,
    LegalIdentifier2 NVARCHAR(100) NULL,

    PrimaryAddressId BIGINT NULL,
    PrimaryContactId BIGINT NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_Company PRIMARY KEY (Id),
    CONSTRAINT UQ_Company_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_Company_Tenant_Code UNIQUE (TenantId, CompanyCode),
    CONSTRAINT UQ_Company_Tenant_PAN UNIQUE (TenantId, PAN),
    CONSTRAINT UQ_Company_Tenant_GSTIN UNIQUE (TenantId, GSTIN),
    CONSTRAINT CK_Company_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_Company_Enterprise FOREIGN KEY (TenantId, EnterpriseId)
        REFERENCES dbo.Enterprise (TenantId, Id),
    CONSTRAINT FK_Company_PrimaryAddress FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address (TenantId, Id),
    CONSTRAINT FK_Company_PrimaryContact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails (TenantId, Id)
);
GO

/* =======================================================================
   BusinessUnit (Vertical: Hospital / Diagnostics / Pharmacy)
   ======================================================================= */
CREATE TABLE dbo.BusinessUnit (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_BusinessUnit_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_BusinessUnit_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_BusinessUnit_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_BusinessUnit_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_BusinessUnit_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_BusinessUnit_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CompanyId BIGINT NOT NULL,
    BusinessUnitCode NVARCHAR(80) NOT NULL,
    BusinessUnitName NVARCHAR(250) NOT NULL,
    BusinessUnitType NVARCHAR(50) NOT NULL,  -- Hospital / Diagnostics / Pharmacy (extensible)

    RegionCode NVARCHAR(30) NULL,
    CountryCode NVARCHAR(10) NULL,

    PrimaryAddressId BIGINT NULL,
    PrimaryContactId BIGINT NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_BusinessUnit PRIMARY KEY (Id),
    CONSTRAINT UQ_BusinessUnit_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_BusinessUnit_Tenant_Code UNIQUE (TenantId, BusinessUnitCode),
    CONSTRAINT CK_BusinessUnit_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_BusinessUnit_Company FOREIGN KEY (TenantId, CompanyId)
        REFERENCES dbo.Company (TenantId, Id),
    CONSTRAINT FK_BusinessUnit_PrimaryAddress FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address (TenantId, Id),
    CONSTRAINT FK_BusinessUnit_PrimaryContact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails (TenantId, Id)
);
GO

/* =======================================================================
   Facility (Hospital / Lab / Pharmacy store location)
   ======================================================================= */
CREATE TABLE dbo.Facility (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Facility_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Facility_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Facility_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Facility_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Facility_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Facility_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BusinessUnitId BIGINT NOT NULL,
    FacilityCode NVARCHAR(80) NOT NULL,
    FacilityName NVARCHAR(250) NOT NULL,
    FacilityType NVARCHAR(50) NOT NULL, -- Hospital / Lab / PharmacyStore

    LicenseDetails NVARCHAR(1000) NULL,
    TimeZoneId NVARCHAR(100) NULL,
    GeoCode NVARCHAR(30) NULL,

    PrimaryAddressId BIGINT NULL,
    PrimaryContactId BIGINT NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_Facility PRIMARY KEY (Id),
    CONSTRAINT UQ_Facility_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_Facility_Tenant_Code UNIQUE (TenantId, FacilityCode),
    CONSTRAINT CK_Facility_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_Facility_BusinessUnit FOREIGN KEY (TenantId, BusinessUnitId)
        REFERENCES dbo.BusinessUnit (TenantId, Id),
    CONSTRAINT FK_Facility_PrimaryAddress FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address (TenantId, Id),
    CONSTRAINT FK_Facility_PrimaryContact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails (TenantId, Id)
);
GO

/* =======================================================================
   Department (OPD / Lab / Pharmacy / Radiology) under a facility
   Note: DepartmentType stored as NVARCHAR to keep schema extensible.
   ======================================================================= */
CREATE TABLE dbo.Department (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Department_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Department_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Department_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Department_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Department_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Department_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    FacilityParentId BIGINT NOT NULL, -- maps to Facility; kept explicit for clarity
    DepartmentCode NVARCHAR(80) NOT NULL,
    DepartmentName NVARCHAR(250) NOT NULL,
    DepartmentType NVARCHAR(50) NOT NULL,

    ParentDepartmentId BIGINT NULL,

    PrimaryAddressId BIGINT NULL,
    PrimaryContactId BIGINT NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_Department PRIMARY KEY (Id),
    CONSTRAINT UQ_Department_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_Department_Tenant_Code UNIQUE (TenantId, DepartmentCode),
    CONSTRAINT CK_Department_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),
    CONSTRAINT CK_Department_Facility_Consistency CHECK (FacilityId = FacilityParentId),

    CONSTRAINT FK_Department_Facility FOREIGN KEY (TenantId, FacilityParentId)
        REFERENCES dbo.Facility (TenantId, Id),
    CONSTRAINT FK_Department_Parent FOREIGN KEY (TenantId, ParentDepartmentId)
        REFERENCES dbo.Department (TenantId, Id),
    CONSTRAINT FK_Department_PrimaryAddress FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address (TenantId, Id),
    CONSTRAINT FK_Department_PrimaryContact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails (TenantId, Id)
);
GO

/* =======================================================================
   EntityConfiguration (key-value based extensibility)
   ======================================================================= */
CREATE TABLE dbo.EntityConfiguration (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EntityConfiguration_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EntityConfiguration_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EntityConfiguration_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EntityConfiguration_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EntityConfiguration_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EntityConfiguration_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EntityType NVARCHAR(80) NOT NULL,  -- Enterprise/Company/BusinessUnit/Facility/Department
    EntityId BIGINT NOT NULL,
    ConfigKey NVARCHAR(200) NOT NULL,
    ConfigValue NVARCHAR(MAX) NULL,
    ConfigDataType NVARCHAR(30) NULL, -- string/int/bool/json
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_EntityConfiguration PRIMARY KEY (Id),
    CONSTRAINT UQ_EntityConfiguration_Key UNIQUE (TenantId, EntityType, EntityId, ConfigKey),
    CONSTRAINT CK_EntityConfiguration_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

/* =======================================================================
   EntityHierarchyMapping (future-proof hierarchy changes)
   ======================================================================= */
CREATE TABLE dbo.EntityHierarchyMapping (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EntityHierarchyMapping_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EntityHierarchyMapping_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EntityHierarchyMapping_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EntityHierarchyMapping_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EntityHierarchyMapping_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EntityHierarchyMapping_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ParentEntityType NVARCHAR(80) NOT NULL,
    ParentEntityId BIGINT NOT NULL,
    ChildEntityType NVARCHAR(80) NOT NULL,
    ChildEntityId BIGINT NOT NULL,
    RelationshipType NVARCHAR(50) NOT NULL CONSTRAINT DF_EntityHierarchyMapping_RelationshipType DEFAULT ('DIRECT'),
    HierarchyLevel SMALLINT NULL,
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_EntityHierarchyMapping PRIMARY KEY (Id),
    CONSTRAINT UQ_EntityHierarchyMapping UNIQUE (TenantId, ParentEntityType, ParentEntityId, ChildEntityType, ChildEntityId, RelationshipType),
    CONSTRAINT CK_EntityHierarchyMapping_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

/* =======================================================================
   Indexes (core)
   ======================================================================= */
CREATE NONCLUSTERED INDEX IX_Enterprise_Tenant_Code
ON dbo.Enterprise (TenantId, EnterpriseCode)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_Company_Tenant_Enterprise
ON dbo.Company (TenantId, EnterpriseId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_BusinessUnit_Tenant_Company
ON dbo.BusinessUnit (TenantId, CompanyId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_Facility_Tenant_BusinessUnit
ON dbo.Facility (TenantId, BusinessUnitId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_Department_Tenant_Facility
ON dbo.Department (TenantId, FacilityParentId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_EntityConfiguration_Tenant_Entity
ON dbo.EntityConfiguration (TenantId, EntityType, EntityId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_EntityHierarchyMapping_Tenant_Parent
ON dbo.EntityHierarchyMapping (TenantId, ParentEntityType, ParentEntityId)
WHERE IsDeleted = 0;
GO

/* =======================================================================
   Sample seed data (optional but helpful)
   ======================================================================= */
DECLARE @TenantId BIGINT = 1;
DECLARE @CreatedBy BIGINT = 1;

-- Seed addresses
INSERT INTO dbo.Address (TenantId, FacilityId, CreatedBy, ModifiedBy, AddressType, Line1, City, StateProvince, PostalCode, CountryCode)
VALUES
(@TenantId, NULL, @CreatedBy, @CreatedBy, 'Registered', '1 Enterprise Plaza', 'Bengaluru', 'Karnataka', '560001', 'IN'),
(@TenantId, NULL, @CreatedBy, @CreatedBy, 'Operational', '10 Health Street', 'Bengaluru', 'Karnataka', '560002', 'IN');

-- Seed contacts
INSERT INTO dbo.ContactDetails (TenantId, FacilityId, CreatedBy, ModifiedBy, ContactType, ContactValue, IsPrimary)
VALUES
(@TenantId, NULL, @CreatedBy, @CreatedBy, 'Email', 'info@enterprise.example', 1),
(@TenantId, NULL, @CreatedBy, @CreatedBy, 'Phone', '+91-80-40000000', 1);

-- Create enterprise/company chain
DECLARE @EnterpriseAddressId BIGINT = (SELECT TOP(1) Id FROM dbo.Address WHERE TenantId = @TenantId AND AddressType = 'Registered' AND IsDeleted = 0 ORDER BY CreatedOn DESC);
DECLARE @EnterpriseContactId BIGINT = (SELECT TOP(1) Id FROM dbo.ContactDetails WHERE TenantId = @TenantId AND ContactType = 'Email' AND IsDeleted = 0 ORDER BY CreatedOn DESC);

INSERT INTO dbo.Enterprise (TenantId, FacilityId, CreatedBy, ModifiedBy, EnterpriseCode, EnterpriseName, RegistrationDetails, PrimaryAddressId, PrimaryContactId, EffectiveFrom)
VALUES (@TenantId, NULL, @CreatedBy, @CreatedBy, 'ENT-ACME', 'Acme Health Group', 'Registration: ABC-123', @EnterpriseAddressId, @EnterpriseContactId, CAST(GETUTCDATE() AS DATE));

DECLARE @EnterpriseId BIGINT = SCOPE_IDENTITY();

-- Company
DECLARE @CompanyAddressId BIGINT = (SELECT TOP(1) Id FROM dbo.Address WHERE TenantId = @TenantId AND AddressType = 'Operational' AND IsDeleted = 0 ORDER BY CreatedOn DESC);
DECLARE @CompanyContactId BIGINT = (SELECT TOP(1) Id FROM dbo.ContactDetails WHERE TenantId = @TenantId AND ContactType = 'Phone' AND IsDeleted = 0 ORDER BY CreatedOn DESC);

INSERT INTO dbo.Company (TenantId, FacilityId, CreatedBy, ModifiedBy, EnterpriseId, CompanyCode, CompanyName, PAN, GSTIN, PrimaryAddressId, PrimaryContactId, EffectiveFrom)
VALUES (@TenantId, NULL, @CreatedBy, @CreatedBy, @EnterpriseId, 'COM-IND', 'Acme Health India Pvt Ltd', 'ABCDE1234F', '29ABCDE1234F1Z5', @CompanyAddressId, @CompanyContactId, CAST(GETUTCDATE() AS DATE));

DECLARE @CompanyId BIGINT = SCOPE_IDENTITY();

-- Business Units
INSERT INTO dbo.BusinessUnit (TenantId, FacilityId, CreatedBy, ModifiedBy, CompanyId, BusinessUnitCode, BusinessUnitName, BusinessUnitType, RegionCode, CountryCode, EffectiveFrom)
VALUES
(@TenantId, NULL, @CreatedBy, @CreatedBy, @CompanyId, 'BU-HOSP', 'Acme Hospitals', 'Hospital', 'SOUTH', 'IN', CAST(GETUTCDATE() AS DATE)),
(@TenantId, NULL, @CreatedBy, @CreatedBy, @CompanyId, 'BU-LABS', 'Acme Diagnostics', 'Diagnostics', 'SOUTH', 'IN', CAST(GETUTCDATE() AS DATE)),
(@TenantId, NULL, @CreatedBy, @CreatedBy, @CompanyId, 'BU-PHAR', 'Acme Pharmacy', 'Pharmacy', 'SOUTH', 'IN', CAST(GETUTCDATE() AS DATE));

DECLARE @HospBUId BIGINT = (SELECT TOP(1) Id FROM dbo.BusinessUnit WHERE TenantId=@TenantId AND BusinessUnitCode='BU-HOSP' ORDER BY CreatedOn DESC);
DECLARE @DiagBUId BIGINT = (SELECT TOP(1) Id FROM dbo.BusinessUnit WHERE TenantId=@TenantId AND BusinessUnitCode='BU-LABS' ORDER BY CreatedOn DESC);
DECLARE @PharBUId BIGINT = (SELECT TOP(1) Id FROM dbo.BusinessUnit WHERE TenantId=@TenantId AND BusinessUnitCode='BU-PHAR' ORDER BY CreatedOn DESC);

-- Facilities
INSERT INTO dbo.Facility (TenantId, FacilityId, CreatedBy, ModifiedBy, BusinessUnitId, FacilityCode, FacilityName, FacilityType, LicenseDetails, EffectiveFrom)
VALUES
(@TenantId, NULL, @CreatedBy, @CreatedBy, @HospBUId, 'FAC-HOSP-BLR-01', 'Acme Hospital Bengaluru', 'Hospital', 'Hospital License 2026', CAST(GETUTCDATE() AS DATE)),
(@TenantId, NULL, @CreatedBy, @CreatedBy, @DiagBUId, 'FAC-LAB-BLR-01', 'Acme Central Lab Bengaluru', 'Lab', 'Lab License 2026', CAST(GETUTCDATE() AS DATE)),
(@TenantId, NULL, @CreatedBy, @CreatedBy, @PharBUId, 'FAC-PHAR-BLR-01', 'Acme Pharmacy Store Bengaluru', 'PharmacyStore', 'Pharmacy License 2026', CAST(GETUTCDATE() AS DATE));

DECLARE @HospFacilityId BIGINT = (SELECT TOP(1) Id FROM dbo.Facility WHERE TenantId=@TenantId AND FacilityCode='FAC-HOSP-BLR-01' ORDER BY CreatedOn DESC);
DECLARE @LabFacilityId BIGINT = (SELECT TOP(1) Id FROM dbo.Facility WHERE TenantId=@TenantId AND FacilityCode='FAC-LAB-BLR-01' ORDER BY CreatedOn DESC);
DECLARE @PharFacilityId BIGINT = (SELECT TOP(1) Id FROM dbo.Facility WHERE TenantId=@TenantId AND FacilityCode='FAC-PHAR-BLR-01' ORDER BY CreatedOn DESC);

-- Departments
INSERT INTO dbo.Department (TenantId, FacilityId, CreatedBy, ModifiedBy, FacilityParentId, DepartmentCode, DepartmentName, DepartmentType, EffectiveFrom)
VALUES
(@TenantId, @HospFacilityId, @CreatedBy, @CreatedBy, @HospFacilityId, 'DEPT-OPD', 'OPD', 'OPD', CAST(GETUTCDATE() AS DATE)),
(@TenantId, @HospFacilityId, @CreatedBy, @CreatedBy, @HospFacilityId, 'DEPT-RAD', 'Radiology', 'Radiology', CAST(GETUTCDATE() AS DATE)),
(@TenantId, @LabFacilityId, @CreatedBy, @CreatedBy, @LabFacilityId, 'DEPT-LAB-CORE', 'Core Lab', 'Lab', CAST(GETUTCDATE() AS DATE)),
(@TenantId, @PharFacilityId, @CreatedBy, @CreatedBy, @PharFacilityId, 'DEPT-PHAR-RET', 'Retail Counter', 'Pharmacy', CAST(GETUTCDATE() AS DATE));
GO

