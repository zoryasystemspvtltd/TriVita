SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  TriVita — Incremental LMS + LIS enhancements (07)
  Run after: 00_EnterpriseHierarchy_Root through 06_Communication.

  Rules:
    - Does not ALTER existing tables from 00–06.
    - Adds new IAM, LMS, LIS, SEC tables aligned with Zorya LMS v0.1 requirements.
    - Multi-tenant: TenantId on all tables; FacilityId where operations are facility-scoped.
    - Status/type enumerations use ReferenceDataValueId per platform standard.
*/

/* =============================================================================
   -- USER MANAGEMENT (IAM: accounts, RBAC, MFA, password reset)
   ============================================================================= */

CREATE TABLE dbo.IAM_UserAccount (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_UserAccount_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_UserAccount_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserAccount_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserAccount_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserAccount_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserAccount_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LoginName NVARCHAR(120) NOT NULL,
    DisplayName NVARCHAR(250) NULL,
    Email NVARCHAR(300) NULL,
    Phone NVARCHAR(50) NULL,
    PasswordHash NVARCHAR(500) NULL,

    PatientId BIGINT NULL,
    DoctorId BIGINT NULL,
    UserStatusReferenceValueId BIGINT NOT NULL,
    LastLoginOn DATETIME2(7) NULL,
    RegistrationSourceReferenceValueId BIGINT NULL,

    CONSTRAINT PK_IAM_UserAccount PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_UserAccount_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_IAM_UserAccount_Tenant_Login UNIQUE (TenantId, LoginName),

    CONSTRAINT FK_IAM_UserAccount_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_IAM_UserAccount_Doctor FOREIGN KEY (TenantId, DoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_IAM_UserAccount_Status FOREIGN KEY (TenantId, UserStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_IAM_UserAccount_RegSource FOREIGN KEY (TenantId, RegistrationSourceReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.IAM_Role (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_Role_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_Role_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_Role_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_Role_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_Role_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_Role_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    RoleCode NVARCHAR(80) NOT NULL,
    RoleName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsSystemRole BIT NOT NULL CONSTRAINT DF_IAM_Role_IsSystem DEFAULT (0),

    CONSTRAINT PK_IAM_Role PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_Role_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_IAM_Role_Tenant_Code UNIQUE (TenantId, RoleCode)
);
GO

CREATE TABLE dbo.IAM_Permission (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_Permission_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_Permission_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_Permission_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_Permission_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_Permission_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_Permission_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PermissionCode NVARCHAR(120) NOT NULL,
    PermissionName NVARCHAR(250) NOT NULL,
    ModuleCode NVARCHAR(80) NULL,
    Description NVARCHAR(500) NULL,

    CONSTRAINT PK_IAM_Permission PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_Permission_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT UQ_IAM_Permission_Tenant_Code UNIQUE (TenantId, PermissionCode)
);
GO

CREATE TABLE dbo.IAM_RolePermission (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_RolePermission_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_RolePermission_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_RolePermission_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_RolePermission_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_RolePermission_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_RolePermission_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    RoleId BIGINT NOT NULL,
    PermissionId BIGINT NOT NULL,

    CONSTRAINT PK_IAM_RolePermission PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_RolePermission_Tenant_Role_Perm UNIQUE (TenantId, RoleId, PermissionId),
    CONSTRAINT UQ_IAM_RolePermission_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_IAM_RolePermission_Role FOREIGN KEY (TenantId, RoleId)
        REFERENCES dbo.IAM_Role(TenantId, Id),
    CONSTRAINT FK_IAM_RolePermission_Permission FOREIGN KEY (TenantId, PermissionId)
        REFERENCES dbo.IAM_Permission(TenantId, Id)
);
GO

CREATE TABLE dbo.IAM_UserRoleAssignment (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_UserRoleAssignment_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_UserRoleAssignment_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserRoleAssignment_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserRoleAssignment_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserRoleAssignment_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserRoleAssignment_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    UserId BIGINT NOT NULL,
    RoleId BIGINT NOT NULL,
    BusinessUnitId BIGINT NULL,
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_IAM_UserRoleAssignment PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_UserRoleAssignment_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_IAM_UserRoleAssignment_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_IAM_UserRoleAssignment_User FOREIGN KEY (TenantId, UserId)
        REFERENCES dbo.IAM_UserAccount(TenantId, Id),
    CONSTRAINT FK_IAM_UserRoleAssignment_Role FOREIGN KEY (TenantId, RoleId)
        REFERENCES dbo.IAM_Role(TenantId, Id),
    CONSTRAINT FK_IAM_UserRoleAssignment_BU FOREIGN KEY (TenantId, BusinessUnitId)
        REFERENCES dbo.BusinessUnit(TenantId, Id)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_IAM_UserRoleAssignment_Tenant_User_Role_NoBU
    ON dbo.IAM_UserRoleAssignment (TenantId, UserId, RoleId)
    WHERE IsDeleted = 0 AND BusinessUnitId IS NULL;
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_IAM_UserRoleAssignment_Tenant_User_Role_BU
    ON dbo.IAM_UserRoleAssignment (TenantId, UserId, RoleId, BusinessUnitId)
    WHERE IsDeleted = 0 AND BusinessUnitId IS NOT NULL;
GO

CREATE TABLE dbo.IAM_UserFacilityScope (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_UserFacilityScope_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_UserFacilityScope_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserFacilityScope_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserFacilityScope_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserFacilityScope_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserFacilityScope_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    UserId BIGINT NOT NULL,
    GrantFacilityId BIGINT NOT NULL,

    CONSTRAINT PK_IAM_UserFacilityScope PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_UserFacilityScope_Tenant_User_GrantFac UNIQUE (TenantId, UserId, GrantFacilityId),
    CONSTRAINT UQ_IAM_UserFacilityScope_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_IAM_UserFacilityScope_User FOREIGN KEY (TenantId, UserId)
        REFERENCES dbo.IAM_UserAccount(TenantId, Id),
    CONSTRAINT FK_IAM_UserFacilityScope_GrantFacility FOREIGN KEY (TenantId, GrantFacilityId)
        REFERENCES dbo.Facility(TenantId, Id)
);
GO

CREATE TABLE dbo.IAM_UserMfaFactor (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_UserMfaFactor_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_UserMfaFactor_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserMfaFactor_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserMfaFactor_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserMfaFactor_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserMfaFactor_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    UserId BIGINT NOT NULL,
    MfaTypeReferenceValueId BIGINT NOT NULL,
    SecretPayload NVARCHAR(MAX) NULL,
    IsVerified BIT NOT NULL CONSTRAINT DF_IAM_UserMfaFactor_IsVerified DEFAULT (0),
    IsPrimary BIT NOT NULL CONSTRAINT DF_IAM_UserMfaFactor_IsPrimary DEFAULT (0),
    LastUsedOn DATETIME2(7) NULL,

    CONSTRAINT PK_IAM_UserMfaFactor PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_UserMfaFactor_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_IAM_UserMfaFactor_User FOREIGN KEY (TenantId, UserId)
        REFERENCES dbo.IAM_UserAccount(TenantId, Id),
    CONSTRAINT FK_IAM_UserMfaFactor_Type FOREIGN KEY (TenantId, MfaTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.IAM_PasswordResetToken (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_PasswordResetToken_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_PasswordResetToken_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_PasswordResetToken_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_PasswordResetToken_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_PasswordResetToken_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_PasswordResetToken_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    UserId BIGINT NOT NULL,
    TokenHash NVARCHAR(256) NOT NULL,
    ExpiresOn DATETIME2(7) NOT NULL,
    ConsumedOn DATETIME2(7) NULL,
    RequestChannelReferenceValueId BIGINT NULL,

    CONSTRAINT PK_IAM_PasswordResetToken PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_PasswordResetToken_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_IAM_PasswordResetToken_User FOREIGN KEY (TenantId, UserId)
        REFERENCES dbo.IAM_UserAccount(TenantId, Id),
    CONSTRAINT FK_IAM_PasswordResetToken_Channel FOREIGN KEY (TenantId, RequestChannelReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_IAM_UserAccount_Tenant_Login
    ON dbo.IAM_UserAccount (TenantId, LoginName) WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_IAM_PasswordResetToken_Tenant_User_Expires
    ON dbo.IAM_PasswordResetToken (TenantId, UserId, ExpiresOn DESC) WHERE IsDeleted = 0 AND ConsumedOn IS NULL;
GO

/* =============================================================================
   -- TEST MANAGEMENT (packages, department/package pricing, analyzer mapping)
   ============================================================================= */

CREATE TABLE dbo.LMS_TestPackage (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_TestPackage_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_TestPackage_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_TestPackage_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_TestPackage_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_TestPackage_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_TestPackage_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PackageCode NVARCHAR(80) NOT NULL,
    PackageName NVARCHAR(250) NOT NULL,
    Description NVARCHAR(1000) NULL,
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_LMS_TestPackage PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_TestPackage_TenantFac_Code UNIQUE (TenantId, FacilityId, PackageCode),
    CONSTRAINT UQ_LMS_TestPackage_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT CK_LMS_TestPackage_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

CREATE TABLE dbo.LMS_TestPackageLine (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_TestPackageLine_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_TestPackageLine_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_TestPackageLine_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_TestPackageLine_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_TestPackageLine_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_TestPackageLine_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestPackageId BIGINT NOT NULL,
    TestMasterId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_LMS_TestPackageLine_Line DEFAULT (0),
    IsOptionalInPackage BIT NOT NULL CONSTRAINT DF_LMS_TestPackageLine_Optional DEFAULT (0),

    CONSTRAINT PK_LMS_TestPackageLine PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_TestPackageLine_TenantFac_Pkg_Line UNIQUE (TenantId, FacilityId, TestPackageId, LineNum),
    CONSTRAINT UQ_LMS_TestPackageLine_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_TestPackageLine_Package FOREIGN KEY (TenantId, FacilityId, TestPackageId)
        REFERENCES dbo.LMS_TestPackage(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_TestPackageLine_Test FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_TestPrice (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_TestPrice_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_TestPrice_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_TestPrice_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_TestPrice_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_TestPrice_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_TestPrice_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestMasterId BIGINT NULL,
    TestPackageId BIGINT NULL,
    DepartmentId BIGINT NULL,
    PriceTierReferenceValueId BIGINT NULL,
    RateAmount DECIMAL(18,4) NOT NULL,
    CurrencyCode NVARCHAR(3) NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_LMS_TestPrice PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_TestPrice_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT CK_LMS_TestPrice_Target CHECK (
        (TestMasterId IS NOT NULL AND TestPackageId IS NULL)
        OR (TestMasterId IS NULL AND TestPackageId IS NOT NULL)
    ),
    CONSTRAINT CK_LMS_TestPrice_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_LMS_TestPrice_Test FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id),
    CONSTRAINT FK_LMS_TestPrice_Package FOREIGN KEY (TenantId, FacilityId, TestPackageId)
        REFERENCES dbo.LMS_TestPackage(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_TestPrice_Department FOREIGN KEY (TenantId, DepartmentId)
        REFERENCES dbo.Department(TenantId, Id),
    CONSTRAINT FK_LMS_TestPrice_Tier FOREIGN KEY (TenantId, PriceTierReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_TestParameterProfile (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_TestParameterProfile_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_TestParameterProfile_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestParameterProfile_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestParameterProfile_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_TestParameterProfile_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_TestParameterProfile_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestParameterId BIGINT NOT NULL,
    MethodReferenceValueId BIGINT NULL,
    CollectionMethodReferenceValueId BIGINT NULL,
    ContainerTypeReferenceValueId BIGINT NULL,
    AnalyzerChannelCode NVARCHAR(80) NULL,
    LoincCode NVARCHAR(40) NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_LIS_TestParameterProfile PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_TestParameterProfile_Tenant_Param UNIQUE (TenantId, TestParameterId),
    CONSTRAINT UQ_LIS_TestParameterProfile_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_LIS_TestParameterProfile_Param FOREIGN KEY (TenantId, TestParameterId)
        REFERENCES dbo.LIS_TestParameters(TenantId, Id),
    CONSTRAINT FK_LIS_TestParameterProfile_Method FOREIGN KEY (TenantId, MethodReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_TestParameterProfile_Collection FOREIGN KEY (TenantId, CollectionMethodReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_TestParameterProfile_Container FOREIGN KEY (TenantId, ContainerTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_AnalyzerResultMap (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_AnalyzerResultMap_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_AnalyzerResultMap_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_AnalyzerResultMap_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_AnalyzerResultMap_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_AnalyzerResultMap_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_AnalyzerResultMap_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EquipmentId BIGINT NOT NULL,
    ExternalTestCode NVARCHAR(80) NOT NULL,
    ExternalParameterCode NVARCHAR(120) NOT NULL CONSTRAINT DF_LIS_AnalyzerResultMap_ExtParam DEFAULT (''),
    TestMasterId BIGINT NOT NULL,
    TestParameterId BIGINT NULL,
    ProtocolReferenceValueId BIGINT NULL,
    UnitOverrideId BIGINT NULL,

    CONSTRAINT PK_LIS_AnalyzerResultMap PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_AnalyzerResultMap_TenantFac_Equip_Ext UNIQUE (TenantId, FacilityId, EquipmentId, ExternalTestCode, ExternalParameterCode),
    CONSTRAINT UQ_LIS_AnalyzerResultMap_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_AnalyzerResultMap_Equipment FOREIGN KEY (TenantId, FacilityId, EquipmentId)
        REFERENCES dbo.Equipment(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_AnalyzerResultMap_Test FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id),
    CONSTRAINT FK_LIS_AnalyzerResultMap_Param FOREIGN KEY (TenantId, TestParameterId)
        REFERENCES dbo.LIS_TestParameters(TenantId, Id),
    CONSTRAINT FK_LIS_AnalyzerResultMap_Protocol FOREIGN KEY (TenantId, ProtocolReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_AnalyzerResultMap_Unit FOREIGN KEY (TenantId, UnitOverrideId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LMS_TestPrice_TenantFac_Dept_Effective
    ON dbo.LMS_TestPrice (TenantId, FacilityId, DepartmentId, EffectiveFrom, EffectiveTo)
    WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LIS_AnalyzerResultMap_TenantFac_Equipment
    ON dbo.LIS_AnalyzerResultMap (TenantId, FacilityId, EquipmentId, ExternalTestCode)
    WHERE IsDeleted = 0;
GO

/* =============================================================================
   -- BILLING (lab-centric invoice, lines, partial payments; wallet)
   -- Note: HMS_PaymentTransactions remains visit-billed; LMS_* covers standalone lab billing.
   ============================================================================= */

CREATE TABLE dbo.LMS_LabInvoiceHeader (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_LabInvoiceHeader_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_LabInvoiceHeader_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LabInvoiceHeader_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LabInvoiceHeader_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LabInvoiceHeader_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LabInvoiceHeader_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    InvoiceNo NVARCHAR(60) NOT NULL,
    LabOrderId BIGINT NOT NULL,
    PatientId BIGINT NOT NULL,
    VisitId BIGINT NULL,
    InvoiceStatusReferenceValueId BIGINT NOT NULL,
    InvoiceDate DATETIME2(7) NOT NULL,

    SubTotal DECIMAL(18,4) NULL,
    TaxTotal DECIMAL(18,4) NULL,
    DiscountTotal DECIMAL(18,4) NULL,
    GrandTotal DECIMAL(18,4) NULL,
    AmountPaid DECIMAL(18,4) NOT NULL CONSTRAINT DF_LMS_LabInvoiceHeader_Paid DEFAULT (0),
    BalanceDue DECIMAL(18,4) NULL,
    CurrencyCode NVARCHAR(3) NULL,

    CONSTRAINT PK_LMS_LabInvoiceHeader PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_LabInvoiceHeader_TenantFac_No UNIQUE (TenantId, FacilityId, InvoiceNo),
    CONSTRAINT UQ_LMS_LabInvoiceHeader_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_LabInvoiceHeader_LabOrder FOREIGN KEY (TenantId, FacilityId, LabOrderId)
        REFERENCES dbo.LIS_LabOrder(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LabInvoiceHeader_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_LMS_LabInvoiceHeader_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LabInvoiceHeader_Status FOREIGN KEY (TenantId, InvoiceStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_LabInvoiceLine (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_LabInvoiceLine_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_LabInvoiceLine_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LabInvoiceLine_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LabInvoiceLine_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LabInvoiceLine_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LabInvoiceLine_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabInvoiceHeaderId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_LMS_LabInvoiceLine_Line DEFAULT (0),
    LineTypeReferenceValueId BIGINT NOT NULL,
    LabOrderItemId BIGINT NULL,
    TestMasterId BIGINT NULL,
    TestPackageId BIGINT NULL,
    Description NVARCHAR(500) NULL,
    Quantity DECIMAL(18,4) NOT NULL CONSTRAINT DF_LMS_LabInvoiceLine_Qty DEFAULT (1),
    UnitPrice DECIMAL(18,4) NULL,
    LineSubTotal DECIMAL(18,4) NULL,
    TaxAmount DECIMAL(18,4) NULL,
    LineTotal DECIMAL(18,4) NULL,

    CONSTRAINT PK_LMS_LabInvoiceLine PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_LabInvoiceLine_TenantFac_Header_Line UNIQUE (TenantId, FacilityId, LabInvoiceHeaderId, LineNum),
    CONSTRAINT UQ_LMS_LabInvoiceLine_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_LabInvoiceLine_Header FOREIGN KEY (TenantId, FacilityId, LabInvoiceHeaderId)
        REFERENCES dbo.LMS_LabInvoiceHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LabInvoiceLine_LineType FOREIGN KEY (TenantId, LineTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_LabInvoiceLine_OrderItem FOREIGN KEY (TenantId, FacilityId, LabOrderItemId)
        REFERENCES dbo.LIS_LabOrderItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LabInvoiceLine_Test FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id),
    CONSTRAINT FK_LMS_LabInvoiceLine_Package FOREIGN KEY (TenantId, FacilityId, TestPackageId)
        REFERENCES dbo.LMS_TestPackage(TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_LabPaymentTransaction (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_LabPaymentTransaction_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_LabPaymentTransaction_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LabPaymentTransaction_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LabPaymentTransaction_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LabPaymentTransaction_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LabPaymentTransaction_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabInvoiceHeaderId BIGINT NOT NULL,
    Amount DECIMAL(18,4) NOT NULL,
    TransactionOn DATETIME2(7) NOT NULL,
    TransactionStatusReferenceValueId BIGINT NOT NULL,
    PaymentModeId BIGINT NULL,
    GatewayProviderReferenceValueId BIGINT NULL,
    ExternalTransactionId NVARCHAR(200) NULL,
    ReferenceNo NVARCHAR(120) NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_LabPaymentTransaction PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_LabPaymentTransaction_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_LabPaymentTransaction_Invoice FOREIGN KEY (TenantId, FacilityId, LabInvoiceHeaderId)
        REFERENCES dbo.LMS_LabInvoiceHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LabPaymentTransaction_Status FOREIGN KEY (TenantId, TransactionStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_LabPaymentTransaction_Mode FOREIGN KEY (TenantId, PaymentModeId)
        REFERENCES dbo.HMS_PaymentModes(TenantId, Id),
    CONSTRAINT FK_LMS_LabPaymentTransaction_Gateway FOREIGN KEY (TenantId, GatewayProviderReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_PatientWalletAccount (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_PatientWalletAccount_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_PatientWalletAccount_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_PatientWalletAccount_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_PatientWalletAccount_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_PatientWalletAccount_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_PatientWalletAccount_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PatientId BIGINT NOT NULL,
    CurrencyCode NVARCHAR(3) NULL,
    CurrentBalance DECIMAL(18,4) NOT NULL CONSTRAINT DF_LMS_PatientWalletAccount_Bal DEFAULT (0),

    CONSTRAINT PK_LMS_PatientWalletAccount PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_PatientWalletAccount_TenantFac_Patient UNIQUE (TenantId, FacilityId, PatientId),
    CONSTRAINT UQ_LMS_PatientWalletAccount_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_PatientWalletAccount_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_PatientWalletTransaction (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_PatientWalletTransaction_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_PatientWalletTransaction_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_PatientWalletTransaction_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_PatientWalletTransaction_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_PatientWalletTransaction_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_PatientWalletTransaction_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PatientWalletAccountId BIGINT NOT NULL,
    AmountDelta DECIMAL(18,4) NOT NULL,
    WalletTxnTypeReferenceValueId BIGINT NOT NULL,
    TransactionOn DATETIME2(7) NOT NULL,
    LabInvoiceHeaderId BIGINT NULL,
    LabPaymentTransactionId BIGINT NULL,
    ReferenceNo NVARCHAR(120) NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_PatientWalletTransaction PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_PatientWalletTransaction_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_PatientWalletTransaction_Account FOREIGN KEY (TenantId, FacilityId, PatientWalletAccountId)
        REFERENCES dbo.LMS_PatientWalletAccount(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_PatientWalletTransaction_TxnType FOREIGN KEY (TenantId, WalletTxnTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_PatientWalletTransaction_Invoice FOREIGN KEY (TenantId, FacilityId, LabInvoiceHeaderId)
        REFERENCES dbo.LMS_LabInvoiceHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_PatientWalletTransaction_Payment FOREIGN KEY (TenantId, FacilityId, LabPaymentTransactionId)
        REFERENCES dbo.LMS_LabPaymentTransaction(TenantId, FacilityId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LMS_LabPaymentTransaction_TenantFac_Invoice_On
    ON dbo.LMS_LabPaymentTransaction (TenantId, FacilityId, LabInvoiceHeaderId, TransactionOn DESC)
    WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LMS_PatientWalletTransaction_TenantFac_Account_On
    ON dbo.LMS_PatientWalletTransaction (TenantId, FacilityId, PatientWalletAccountId, TransactionOn DESC)
    WHERE IsDeleted = 0;
GO

/* =============================================================================
   -- REFERRAL & FINANCIALS (referral master, fee rules, ledger, settlements)
   ============================================================================= */

CREATE TABLE dbo.LMS_ReferralDoctorProfile (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReferralDoctorProfile_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReferralDoctorProfile_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralDoctorProfile_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralDoctorProfile_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralDoctorProfile_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralDoctorProfile_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReferralCode NVARCHAR(80) NOT NULL,
    DisplayName NVARCHAR(250) NOT NULL,
    LinkedDoctorId BIGINT NULL,
    HospitalAffiliation NVARCHAR(300) NULL,
    PrimaryContactId BIGINT NULL,
    PrimaryAddressId BIGINT NULL,
    ReferralTypeReferenceValueId BIGINT NULL,

    CONSTRAINT PK_LMS_ReferralDoctorProfile PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReferralDoctorProfile_Tenant_Code UNIQUE (TenantId, ReferralCode),
    CONSTRAINT UQ_LMS_ReferralDoctorProfile_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_LMS_ReferralDoctorProfile_Doctor FOREIGN KEY (TenantId, LinkedDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_LMS_ReferralDoctorProfile_Contact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails(TenantId, Id),
    CONSTRAINT FK_LMS_ReferralDoctorProfile_Address FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address(TenantId, Id),
    CONSTRAINT FK_LMS_ReferralDoctorProfile_Type FOREIGN KEY (TenantId, ReferralTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_ReferralFeeRule (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReferralFeeRule_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReferralFeeRule_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralFeeRule_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralFeeRule_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralFeeRule_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralFeeRule_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReferralDoctorProfileId BIGINT NOT NULL,
    FeeModeReferenceValueId BIGINT NOT NULL,
    FeeValue DECIMAL(18,4) NOT NULL,
    ApplyScopeReferenceValueId BIGINT NOT NULL,
    TestMasterId BIGINT NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_LMS_ReferralFeeRule PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReferralFeeRule_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT CK_LMS_ReferralFeeRule_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_LMS_ReferralFeeRule_Referral FOREIGN KEY (TenantId, ReferralDoctorProfileId)
        REFERENCES dbo.LMS_ReferralDoctorProfile(TenantId, Id),
    CONSTRAINT FK_LMS_ReferralFeeRule_FeeMode FOREIGN KEY (TenantId, FeeModeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_ReferralFeeRule_Scope FOREIGN KEY (TenantId, ApplyScopeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_ReferralFeeRule_Test FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_ReferralFeeLedger (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReferralFeeLedger_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReferralFeeLedger_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralFeeLedger_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralFeeLedger_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralFeeLedger_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralFeeLedger_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReferralDoctorProfileId BIGINT NOT NULL,
    LabInvoiceHeaderId BIGINT NOT NULL,
    LabInvoiceLineId BIGINT NULL,
    LabOrderItemId BIGINT NULL,
    FeeAmount DECIMAL(18,4) NOT NULL,
    LedgerStatusReferenceValueId BIGINT NOT NULL,
    AccruedOn DATETIME2(7) NOT NULL,

    CONSTRAINT PK_LMS_ReferralFeeLedger PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReferralFeeLedger_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_ReferralFeeLedger_Referral FOREIGN KEY (TenantId, ReferralDoctorProfileId)
        REFERENCES dbo.LMS_ReferralDoctorProfile(TenantId, Id),
    CONSTRAINT FK_LMS_ReferralFeeLedger_Invoice FOREIGN KEY (TenantId, FacilityId, LabInvoiceHeaderId)
        REFERENCES dbo.LMS_LabInvoiceHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReferralFeeLedger_InvoiceLine FOREIGN KEY (TenantId, FacilityId, LabInvoiceLineId)
        REFERENCES dbo.LMS_LabInvoiceLine(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReferralFeeLedger_OrderItem FOREIGN KEY (TenantId, FacilityId, LabOrderItemId)
        REFERENCES dbo.LIS_LabOrderItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReferralFeeLedger_Status FOREIGN KEY (TenantId, LedgerStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_ReferralSettlement (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReferralSettlement_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReferralSettlement_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralSettlement_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralSettlement_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralSettlement_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralSettlement_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SettlementNo NVARCHAR(60) NOT NULL,
    ReferralDoctorProfileId BIGINT NOT NULL,
    PeriodStartOn DATE NOT NULL,
    PeriodEndOn DATE NOT NULL,
    TotalSettledAmount DECIMAL(18,4) NOT NULL,
    SettlementStatusReferenceValueId BIGINT NOT NULL,
    SettledOn DATETIME2(7) NULL,
    PaymentReferenceNo NVARCHAR(120) NULL,

    CONSTRAINT PK_LMS_ReferralSettlement PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReferralSettlement_TenantFac_No UNIQUE (TenantId, FacilityId, SettlementNo),
    CONSTRAINT UQ_LMS_ReferralSettlement_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT CK_LMS_ReferralSettlement_Period CHECK (PeriodEndOn >= PeriodStartOn),

    CONSTRAINT FK_LMS_ReferralSettlement_Referral FOREIGN KEY (TenantId, ReferralDoctorProfileId)
        REFERENCES dbo.LMS_ReferralDoctorProfile(TenantId, Id),
    CONSTRAINT FK_LMS_ReferralSettlement_Status FOREIGN KEY (TenantId, SettlementStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_ReferralSettlementLine (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReferralSettlementLine_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReferralSettlementLine_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralSettlementLine_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralSettlementLine_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReferralSettlementLine_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReferralSettlementLine_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReferralSettlementId BIGINT NOT NULL,
    ReferralFeeLedgerId BIGINT NOT NULL,
    AppliedAmount DECIMAL(18,4) NOT NULL,

    CONSTRAINT PK_LMS_ReferralSettlementLine PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReferralSettlementLine_TenantFac_Ledger UNIQUE (TenantId, FacilityId, ReferralFeeLedgerId),
    CONSTRAINT UQ_LMS_ReferralSettlementLine_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_ReferralSettlementLine_Settlement FOREIGN KEY (TenantId, FacilityId, ReferralSettlementId)
        REFERENCES dbo.LMS_ReferralSettlement(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReferralSettlementLine_Ledger FOREIGN KEY (TenantId, FacilityId, ReferralFeeLedgerId)
        REFERENCES dbo.LMS_ReferralFeeLedger(TenantId, FacilityId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LMS_ReferralFeeLedger_TenantFac_Referral_Status
    ON dbo.LMS_ReferralFeeLedger (TenantId, FacilityId, ReferralDoctorProfileId, LedgerStatusReferenceValueId, AccruedOn DESC)
    WHERE IsDeleted = 0;
GO

/* =============================================================================
   -- B2B / CREDIT (partner master, contract rates, limits, utilization ledger)
   ============================================================================= */

CREATE TABLE dbo.LMS_B2BPartner (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_B2BPartner_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_B2BPartner_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartner_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartner_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartner_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartner_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PartnerCode NVARCHAR(80) NOT NULL,
    PartnerName NVARCHAR(250) NOT NULL,
    PartnerCategoryReferenceValueId BIGINT NULL,
    PrimaryAddressId BIGINT NULL,
    PrimaryContactId BIGINT NULL,
    ContractReference NVARCHAR(200) NULL,

    CONSTRAINT PK_LMS_B2BPartner PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_B2BPartner_Tenant_Code UNIQUE (TenantId, PartnerCode),
    CONSTRAINT UQ_LMS_B2BPartner_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_LMS_B2BPartner_Category FOREIGN KEY (TenantId, PartnerCategoryReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_B2BPartner_Address FOREIGN KEY (TenantId, PrimaryAddressId)
        REFERENCES dbo.Address(TenantId, Id),
    CONSTRAINT FK_LMS_B2BPartner_Contact FOREIGN KEY (TenantId, PrimaryContactId)
        REFERENCES dbo.ContactDetails(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_B2BPartnerTestRate (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_B2BPartnerTestRate_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_B2BPartnerTestRate_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartnerTestRate_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartnerTestRate_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartnerTestRate_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartnerTestRate_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    B2BPartnerId BIGINT NOT NULL,
    TestMasterId BIGINT NOT NULL,
    RateAmount DECIMAL(18,4) NULL,
    DiscountPercent DECIMAL(9,4) NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    ContractDocumentRef NVARCHAR(200) NULL,

    CONSTRAINT PK_LMS_B2BPartnerTestRate PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_B2BPartnerTestRate_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT CK_LMS_B2BPartnerTestRate_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),
    CONSTRAINT CK_LMS_B2BPartnerTestRate_AmountOrPct CHECK (RateAmount IS NOT NULL OR DiscountPercent IS NOT NULL),

    CONSTRAINT FK_LMS_B2BPartnerTestRate_Partner FOREIGN KEY (TenantId, B2BPartnerId)
        REFERENCES dbo.LMS_B2BPartner(TenantId, Id),
    CONSTRAINT FK_LMS_B2BPartnerTestRate_Test FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_B2BPartnerCreditProfile (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_B2BPartnerCreditProfile_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_B2BPartnerCreditProfile_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartnerCreditProfile_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartnerCreditProfile_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartnerCreditProfile_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartnerCreditProfile_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    B2BPartnerId BIGINT NOT NULL,
    CreditLimitAmount DECIMAL(18,4) NOT NULL,
    CreditCurrencyCode NVARCHAR(3) NULL,
    PaymentTermsDays INT NULL,
    GracePeriodDays INT NULL,
    UtilizedAmount DECIMAL(18,4) NOT NULL CONSTRAINT DF_LMS_B2BPartnerCreditProfile_Util DEFAULT (0),

    CONSTRAINT PK_LMS_B2BPartnerCreditProfile PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_B2BPartnerCreditProfile_TenantFac_Partner UNIQUE (TenantId, FacilityId, B2BPartnerId),
    CONSTRAINT UQ_LMS_B2BPartnerCreditProfile_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_B2BPartnerCreditProfile_Partner FOREIGN KEY (TenantId, B2BPartnerId)
        REFERENCES dbo.LMS_B2BPartner(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_B2BCreditLedger (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_B2BCreditLedger_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_B2BCreditLedger_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BCreditLedger_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BCreditLedger_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BCreditLedger_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BCreditLedger_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    B2BPartnerCreditProfileId BIGINT NOT NULL,
    MovementTypeReferenceValueId BIGINT NOT NULL,
    AmountDelta DECIMAL(18,4) NOT NULL,
    PostedOn DATETIME2(7) NOT NULL,
    LabInvoiceHeaderId BIGINT NULL,
    ReferenceNo NVARCHAR(120) NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_B2BCreditLedger PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_B2BCreditLedger_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_B2BCreditLedger_Profile FOREIGN KEY (TenantId, FacilityId, B2BPartnerCreditProfileId)
        REFERENCES dbo.LMS_B2BPartnerCreditProfile(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_B2BCreditLedger_MovementType FOREIGN KEY (TenantId, MovementTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_B2BCreditLedger_Invoice FOREIGN KEY (TenantId, FacilityId, LabInvoiceHeaderId)
        REFERENCES dbo.LMS_LabInvoiceHeader(TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_B2BPartnerBillingStatement (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatement_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatement_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatement_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatement_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatement_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatement_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    StatementNo NVARCHAR(60) NOT NULL,
    B2BPartnerId BIGINT NOT NULL,
    PeriodStartOn DATE NOT NULL,
    PeriodEndOn DATE NOT NULL,
    TotalAmount DECIMAL(18,4) NOT NULL,
    StatementStatusReferenceValueId BIGINT NOT NULL,
    IssuedOn DATETIME2(7) NULL,

    CONSTRAINT PK_LMS_B2BPartnerBillingStatement PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_B2BPartnerBillingStatement_TenantFac_No UNIQUE (TenantId, FacilityId, StatementNo),
    CONSTRAINT UQ_LMS_B2BPartnerBillingStatement_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT CK_LMS_B2BPartnerBillingStatement_Period CHECK (PeriodEndOn >= PeriodStartOn),

    CONSTRAINT FK_LMS_B2BPartnerBillingStatement_Partner FOREIGN KEY (TenantId, B2BPartnerId)
        REFERENCES dbo.LMS_B2BPartner(TenantId, Id),
    CONSTRAINT FK_LMS_B2BPartnerBillingStatement_Status FOREIGN KEY (TenantId, StatementStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_B2BPartnerBillingStatementLine (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatementLine_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatementLine_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatementLine_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatementLine_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatementLine_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatementLine_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PartnerBillingStatementId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_LMS_B2BPartnerBillingStatementLine_Line DEFAULT (0),
    LabInvoiceHeaderId BIGINT NULL,
    Description NVARCHAR(500) NULL,
    LineAmount DECIMAL(18,4) NOT NULL,

    CONSTRAINT PK_LMS_B2BPartnerBillingStatementLine PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_B2BPartnerBillingStatementLine_TenantFac_HdrLine UNIQUE (TenantId, FacilityId, PartnerBillingStatementId, LineNum),
    CONSTRAINT UQ_LMS_B2BPartnerBillingStatementLine_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_B2BPartnerBillingStatementLine_Header FOREIGN KEY (TenantId, FacilityId, PartnerBillingStatementId)
        REFERENCES dbo.LMS_B2BPartnerBillingStatement(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_B2BPartnerBillingStatementLine_Invoice FOREIGN KEY (TenantId, FacilityId, LabInvoiceHeaderId)
        REFERENCES dbo.LMS_LabInvoiceHeader(TenantId, FacilityId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LMS_B2BPartnerTestRate_TenantFac_Partner_Test
    ON dbo.LMS_B2BPartnerTestRate (TenantId, FacilityId, B2BPartnerId, TestMasterId, EffectiveFrom, EffectiveTo)
    WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LMS_B2BCreditLedger_TenantFac_Profile_Posted
    ON dbo.LMS_B2BCreditLedger (TenantId, FacilityId, B2BPartnerCreditProfileId, PostedOn DESC)
    WHERE IsDeleted = 0;
GO

/* =============================================================================
   -- INVENTORY (reagent master, batches, test mapping, consumption log)
   ============================================================================= */

CREATE TABLE dbo.LMS_ReagentMaster (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReagentMaster_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReagentMaster_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReagentMaster_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReagentMaster_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReagentMaster_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReagentMaster_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReagentCode NVARCHAR(80) NOT NULL,
    ReagentName NVARCHAR(250) NOT NULL,
    DefaultUnitId BIGINT NULL,
    StorageNotes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_ReagentMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReagentMaster_TenantFac_Code UNIQUE (TenantId, FacilityId, ReagentCode),
    CONSTRAINT UQ_LMS_ReagentMaster_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_ReagentMaster_Unit FOREIGN KEY (TenantId, DefaultUnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_ReagentBatch (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReagentBatch_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReagentBatch_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReagentBatch_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReagentBatch_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReagentBatch_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReagentBatch_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReagentMasterId BIGINT NOT NULL,
    LotNo NVARCHAR(120) NOT NULL,
    ExpiryDate DATE NULL,
    ReceivedOn DATE NULL,
    LabInventoryId BIGINT NULL,
    OpeningQuantity DECIMAL(18,4) NULL,
    CurrentQuantity DECIMAL(18,4) NULL,

    CONSTRAINT PK_LMS_ReagentBatch PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReagentBatch_TenantFac_Master_Lot UNIQUE (TenantId, FacilityId, ReagentMasterId, LotNo),
    CONSTRAINT UQ_LMS_ReagentBatch_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_ReagentBatch_Master FOREIGN KEY (TenantId, FacilityId, ReagentMasterId)
        REFERENCES dbo.LMS_ReagentMaster(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReagentBatch_LabInventory FOREIGN KEY (TenantId, FacilityId, LabInventoryId)
        REFERENCES dbo.LabInventory(TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_TestReagentMap (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_TestReagentMap_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_TestReagentMap_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_TestReagentMap_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_TestReagentMap_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_TestReagentMap_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_TestReagentMap_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TestMasterId BIGINT NOT NULL,
    ReagentMasterId BIGINT NOT NULL,
    QuantityPerTest DECIMAL(18,4) NOT NULL,
    UnitId BIGINT NULL,

    CONSTRAINT PK_LMS_TestReagentMap PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_TestReagentMap_TenantFac_Test_Reagent UNIQUE (TenantId, FacilityId, TestMasterId, ReagentMasterId),
    CONSTRAINT UQ_LMS_TestReagentMap_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_TestReagentMap_Test FOREIGN KEY (TenantId, TestMasterId)
        REFERENCES dbo.LIS_TestMaster(TenantId, Id),
    CONSTRAINT FK_LMS_TestReagentMap_Reagent FOREIGN KEY (TenantId, FacilityId, ReagentMasterId)
        REFERENCES dbo.LMS_ReagentMaster(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_TestReagentMap_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_ReagentConsumptionLog (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReagentConsumptionLog_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReagentConsumptionLog_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReagentConsumptionLog_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReagentConsumptionLog_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReagentConsumptionLog_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReagentConsumptionLog_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReagentBatchId BIGINT NOT NULL,
    QuantityConsumed DECIMAL(18,4) NOT NULL,
    ConsumedOn DATETIME2(7) NOT NULL,
    LabOrderItemId BIGINT NULL,
    WorkQueueId BIGINT NULL,
    ConsumptionReasonReferenceValueId BIGINT NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_ReagentConsumptionLog PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReagentConsumptionLog_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_ReagentConsumptionLog_Batch FOREIGN KEY (TenantId, FacilityId, ReagentBatchId)
        REFERENCES dbo.LMS_ReagentBatch(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReagentConsumptionLog_OrderItem FOREIGN KEY (TenantId, FacilityId, LabOrderItemId)
        REFERENCES dbo.LIS_LabOrderItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReagentConsumptionLog_WorkQueue FOREIGN KEY (TenantId, FacilityId, WorkQueueId)
        REFERENCES dbo.WorkQueue(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReagentConsumptionLog_Reason FOREIGN KEY (TenantId, ConsumptionReasonReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LMS_ReagentConsumptionLog_TenantFac_Batch_On
    ON dbo.LMS_ReagentConsumptionLog (TenantId, FacilityId, ReagentBatchId, ConsumedOn DESC)
    WHERE IsDeleted = 0;
GO

/* =============================================================================
   -- SAMPLE & WORKFLOW (barcode/QR, order context, lifecycle / TAT)
   ============================================================================= */

CREATE TABLE dbo.LMS_LabOrderContext (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_LabOrderContext_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_LabOrderContext_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LabOrderContext_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LabOrderContext_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_LabOrderContext_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_LabOrderContext_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderId BIGINT NOT NULL,
    B2BPartnerId BIGINT NULL,
    ReferralDoctorProfileId BIGINT NULL,
    SampleSourceReferenceValueId BIGINT NULL,
    BookingChannelReferenceValueId BIGINT NULL,
    ExpectedReportOn DATETIME2(7) NULL,

    CONSTRAINT PK_LMS_LabOrderContext PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_LabOrderContext_TenantFac_Order UNIQUE (TenantId, FacilityId, LabOrderId),
    CONSTRAINT UQ_LMS_LabOrderContext_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_LabOrderContext_LabOrder FOREIGN KEY (TenantId, FacilityId, LabOrderId)
        REFERENCES dbo.LIS_LabOrder(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_LabOrderContext_Partner FOREIGN KEY (TenantId, B2BPartnerId)
        REFERENCES dbo.LMS_B2BPartner(TenantId, Id),
    CONSTRAINT FK_LMS_LabOrderContext_Referral FOREIGN KEY (TenantId, ReferralDoctorProfileId)
        REFERENCES dbo.LMS_ReferralDoctorProfile(TenantId, Id),
    CONSTRAINT FK_LMS_LabOrderContext_SampleSource FOREIGN KEY (TenantId, SampleSourceReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_LabOrderContext_BookingChannel FOREIGN KEY (TenantId, BookingChannelReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_SampleBarcode (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_SampleBarcode_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_SampleBarcode_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleBarcode_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleBarcode_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleBarcode_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleBarcode_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SampleCollectionId BIGINT NOT NULL,
    BarcodeValue NVARCHAR(120) NOT NULL,
    QrPayload NVARCHAR(MAX) NULL,
    IdentifierTypeReferenceValueId BIGINT NULL,
    PrintedOn DATETIME2(7) NULL,

    CONSTRAINT PK_LIS_SampleBarcode PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_SampleBarcode_TenantFac_Barcode UNIQUE (TenantId, FacilityId, BarcodeValue),
    CONSTRAINT UQ_LIS_SampleBarcode_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_SampleBarcode_Collection FOREIGN KEY (TenantId, FacilityId, SampleCollectionId)
        REFERENCES dbo.LIS_SampleCollection(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_SampleBarcode_Type FOREIGN KEY (TenantId, IdentifierTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_SampleLifecycleEvent (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_SampleLifecycleEvent_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_SampleLifecycleEvent_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleLifecycleEvent_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleLifecycleEvent_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_SampleLifecycleEvent_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_SampleLifecycleEvent_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    SampleCollectionId BIGINT NOT NULL,
    LabOrderItemId BIGINT NULL,
    EventTypeReferenceValueId BIGINT NOT NULL,
    EventOn DATETIME2(7) NOT NULL,
    PlannedDueOn DATETIME2(7) NULL,
    TatBreached BIT NOT NULL CONSTRAINT DF_LIS_SampleLifecycleEvent_TatBreach DEFAULT (0),
    LocationDepartmentId BIGINT NULL,
    EventNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_LIS_SampleLifecycleEvent PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_SampleLifecycleEvent_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_SampleLifecycleEvent_Collection FOREIGN KEY (TenantId, FacilityId, SampleCollectionId)
        REFERENCES dbo.LIS_SampleCollection(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_SampleLifecycleEvent_OrderItem FOREIGN KEY (TenantId, FacilityId, LabOrderItemId)
        REFERENCES dbo.LIS_LabOrderItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_SampleLifecycleEvent_EventType FOREIGN KEY (TenantId, EventTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LIS_SampleLifecycleEvent_Dept FOREIGN KEY (TenantId, LocationDepartmentId)
        REFERENCES dbo.Department(TenantId, Id)
);
GO

CREATE TABLE dbo.LMS_ReportPaymentGate (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_ReportPaymentGate_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_ReportPaymentGate_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReportPaymentGate_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReportPaymentGate_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_ReportPaymentGate_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_ReportPaymentGate_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReportHeaderId BIGINT NOT NULL,
    LabInvoiceHeaderId BIGINT NOT NULL,
    MinimumPaidPercent DECIMAL(9,4) NOT NULL CONSTRAINT DF_LMS_ReportPaymentGate_MinPct DEFAULT (100),
    IsReleased BIT NOT NULL CONSTRAINT DF_LMS_ReportPaymentGate_Released DEFAULT (0),
    ReleasedOn DATETIME2(7) NULL,
    ReleaseReasonReferenceValueId BIGINT NULL,

    CONSTRAINT PK_LMS_ReportPaymentGate PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_ReportPaymentGate_TenantFac_Report UNIQUE (TenantId, FacilityId, ReportHeaderId),
    CONSTRAINT UQ_LMS_ReportPaymentGate_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_ReportPaymentGate_Report FOREIGN KEY (TenantId, FacilityId, ReportHeaderId)
        REFERENCES dbo.LIS_ReportHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReportPaymentGate_Invoice FOREIGN KEY (TenantId, FacilityId, LabInvoiceHeaderId)
        REFERENCES dbo.LMS_LabInvoiceHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LMS_ReportPaymentGate_ReleaseReason FOREIGN KEY (TenantId, ReleaseReasonReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.LIS_ReportDeliveryOtp (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_ReportDeliveryOtp_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_ReportDeliveryOtp_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ReportDeliveryOtp_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ReportDeliveryOtp_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ReportDeliveryOtp_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ReportDeliveryOtp_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReportHeaderId BIGINT NOT NULL,
    OtpHash NVARCHAR(256) NOT NULL,
    ExpiresOn DATETIME2(7) NOT NULL,
    ConsumedOn DATETIME2(7) NULL,
    DeliveryChannelReferenceValueId BIGINT NULL,

    CONSTRAINT PK_LIS_ReportDeliveryOtp PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_ReportDeliveryOtp_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_ReportDeliveryOtp_Report FOREIGN KEY (TenantId, FacilityId, ReportHeaderId)
        REFERENCES dbo.LIS_ReportHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_ReportDeliveryOtp_Channel FOREIGN KEY (TenantId, DeliveryChannelReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LIS_SampleLifecycleEvent_TenantFac_Collection_On
    ON dbo.LIS_SampleLifecycleEvent (TenantId, FacilityId, SampleCollectionId, EventOn DESC)
    WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LIS_ReportDeliveryOtp_TenantFac_Report_Expires
    ON dbo.LIS_ReportDeliveryOtp (TenantId, FacilityId, ReportHeaderId, ExpiresOn DESC)
    WHERE IsDeleted = 0 AND ConsumedOn IS NULL;
GO

CREATE TABLE dbo.LIS_ReportLockState (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LIS_ReportLockState_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LIS_ReportLockState_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ReportLockState_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ReportLockState_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LIS_ReportLockState_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LIS_ReportLockState_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReportHeaderId BIGINT NOT NULL,
    IsLocked BIT NOT NULL CONSTRAINT DF_LIS_ReportLockState_IsLocked DEFAULT (0),
    LockedOn DATETIME2(7) NULL,
    LockedByUserId BIGINT NULL,
    LockReasonReferenceValueId BIGINT NULL,

    CONSTRAINT PK_LIS_ReportLockState PRIMARY KEY (Id),
    CONSTRAINT UQ_LIS_ReportLockState_TenantFac_Report UNIQUE (TenantId, FacilityId, ReportHeaderId),
    CONSTRAINT UQ_LIS_ReportLockState_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LIS_ReportLockState_Report FOREIGN KEY (TenantId, FacilityId, ReportHeaderId)
        REFERENCES dbo.LIS_ReportHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_LIS_ReportLockState_User FOREIGN KEY (TenantId, LockedByUserId)
        REFERENCES dbo.IAM_UserAccount(TenantId, Id),
    CONSTRAINT FK_LIS_ReportLockState_Reason FOREIGN KEY (TenantId, LockReasonReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =============================================================================
   -- REPORTING & ANALYTICS (finance ledger + daily rollups)
   ============================================================================= */

CREATE TABLE dbo.LMS_FinanceLedgerEntry (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_FinanceLedgerEntry_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_FinanceLedgerEntry_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_FinanceLedgerEntry_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_FinanceLedgerEntry_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_FinanceLedgerEntry_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_FinanceLedgerEntry_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EntryDate DATE NOT NULL,
    AccountCategoryReferenceValueId BIGINT NOT NULL,
    SourceTypeReferenceValueId BIGINT NOT NULL,
    SourceId BIGINT NOT NULL,
    Amount DECIMAL(18,4) NOT NULL,
    DebitCreditReferenceValueId BIGINT NULL,
    PatientId BIGINT NULL,
    LabOrderId BIGINT NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_LMS_FinanceLedgerEntry PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_FinanceLedgerEntry_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_FinanceLedgerEntry_AccountCat FOREIGN KEY (TenantId, AccountCategoryReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_FinanceLedgerEntry_SourceType FOREIGN KEY (TenantId, SourceTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_FinanceLedgerEntry_DebitCredit FOREIGN KEY (TenantId, DebitCreditReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LMS_FinanceLedgerEntry_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_LMS_FinanceLedgerEntry_LabOrder FOREIGN KEY (TenantId, FacilityId, LabOrderId)
        REFERENCES dbo.LIS_LabOrder(TenantId, FacilityId, Id)
);
GO

CREATE TABLE dbo.LMS_AnalyticsDailyFacilityRollup (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    StatDate DATE NOT NULL,
    LabOrderCount INT NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_Orders DEFAULT (0),
    ReportIssuedCount INT NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_Reports DEFAULT (0),
    GrossRevenue DECIMAL(18,4) NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_Gross DEFAULT (0),
    DiscountTotal DECIMAL(18,4) NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_Disc DEFAULT (0),
    NetRevenue DECIMAL(18,4) NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_Net DEFAULT (0),
    ReferralFeeAccrued DECIMAL(18,4) NOT NULL CONSTRAINT DF_LMS_AnalyticsDailyFacilityRollup_Ref DEFAULT (0),
    AvgTatMinutes INT NULL,

    CONSTRAINT PK_LMS_AnalyticsDailyFacilityRollup PRIMARY KEY (Id),
    CONSTRAINT UQ_LMS_AnalyticsDailyFacilityRollup_TenantFac_Date UNIQUE (TenantId, FacilityId, StatDate),
    CONSTRAINT UQ_LMS_AnalyticsDailyFacilityRollup_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LMS_AnalyticsDailyFacilityRollup_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_LMS_FinanceLedgerEntry_TenantFac_Date_Category
    ON dbo.LMS_FinanceLedgerEntry (TenantId, FacilityId, EntryDate, AccountCategoryReferenceValueId)
    WHERE IsDeleted = 0;
GO

/* =============================================================================
   -- AUDIT & SECURITY (data change log, session / login activity)
   ============================================================================= */

CREATE TABLE dbo.SEC_DataChangeAuditLog (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_SEC_DataChangeAuditLog_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_SEC_DataChangeAuditLog_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_SEC_DataChangeAuditLog_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_SEC_DataChangeAuditLog_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_SEC_DataChangeAuditLog_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_SEC_DataChangeAuditLog_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    UserId BIGINT NULL,
    ActionTypeReferenceValueId BIGINT NOT NULL,
    EntitySchema NVARCHAR(50) NOT NULL,
    EntityName NVARCHAR(120) NOT NULL,
    EntityKeyJson NVARCHAR(500) NULL,
    ChangeSummary NVARCHAR(2000) NULL,
    CorrelationId NVARCHAR(80) NULL,
    ClientIp NVARCHAR(64) NULL,
    UserAgent NVARCHAR(500) NULL,

    CONSTRAINT PK_SEC_DataChangeAuditLog PRIMARY KEY (Id),
    CONSTRAINT UQ_SEC_DataChangeAuditLog_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_SEC_DataChangeAuditLog_User FOREIGN KEY (TenantId, UserId)
        REFERENCES dbo.IAM_UserAccount(TenantId, Id),
    CONSTRAINT FK_SEC_DataChangeAuditLog_Action FOREIGN KEY (TenantId, ActionTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE TABLE dbo.IAM_UserSessionActivity (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_IAM_UserSessionActivity_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_UserSessionActivity_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserSessionActivity_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserSessionActivity_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserSessionActivity_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserSessionActivity_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    UserId BIGINT NOT NULL,
    ActivityTypeReferenceValueId BIGINT NOT NULL,
    ActivityOn DATETIME2(7) NOT NULL,
    SessionTokenHash NVARCHAR(256) NULL,
    ClientIp NVARCHAR(64) NULL,
    UserAgent NVARCHAR(500) NULL,
    Success BIT NOT NULL CONSTRAINT DF_IAM_UserSessionActivity_Success DEFAULT (1),
    FailureReason NVARCHAR(500) NULL,

    CONSTRAINT PK_IAM_UserSessionActivity PRIMARY KEY (Id),
    CONSTRAINT UQ_IAM_UserSessionActivity_TenantId_Id UNIQUE (TenantId, Id),

    CONSTRAINT FK_IAM_UserSessionActivity_User FOREIGN KEY (TenantId, UserId)
        REFERENCES dbo.IAM_UserAccount(TenantId, Id),
    CONSTRAINT FK_IAM_UserSessionActivity_Type FOREIGN KEY (TenantId, ActivityTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_SEC_DataChangeAuditLog_Tenant_Entity_On
    ON dbo.SEC_DataChangeAuditLog (TenantId, EntityName, CreatedOn DESC)
    WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_IAM_UserSessionActivity_Tenant_User_On
    ON dbo.IAM_UserSessionActivity (TenantId, UserId, ActivityOn DESC)
    WHERE IsDeleted = 0;
GO

/*
  End of 07_LMS_LIS_Enhancement.sql

  Cross-check (vs 00–06):
    - No table name collisions with existing dbo.* objects.
    - HMS_PaymentTransactions unchanged; lab partial payments use LMS_LabPaymentTransaction.
    - Populate ReferenceDataDefinition/Value for IAM/LMS/LIS/SEC status codes before go-live.
*/
