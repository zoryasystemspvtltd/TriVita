SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  TriVita — Incremental feature enhancement (script 09)
  Rules:
    - Does NOT ALTER or DROP existing tables/columns.
    - Only CREATE TABLE for extension / cross-cutting capabilities.
  Dependencies:
    - 00_EnterpriseHierarchy_Root.sql (Enterprise, Facility FKs where used)
  Optional (monolithic dbo only):
    - If LIS tables exist in the same database, you may add FKs from EXT_LabCriticalValueEscalation
      to LIS_LabResults / LIS_LabOrderItems via a follow-up script; this script keeps logical IDs
      without cross-module FKs for split-database deployments.
*/

/* =======================================================================
   B2B / corporate & insurance contracts (enterprise scope)
   ======================================================================= */
CREATE TABLE dbo.EXT_EnterpriseB2BContract (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EXT_B2B_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EXT_B2B_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_B2B_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EXT_B2B_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_B2B_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EXT_B2B_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EnterpriseId BIGINT NOT NULL,
    PartnerType NVARCHAR(50) NOT NULL,
    PartnerName NVARCHAR(250) NOT NULL,
    ContractCode NVARCHAR(80) NOT NULL,
    TermsJson NVARCHAR(MAX) NULL,
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_EXT_EnterpriseB2BContract PRIMARY KEY (Id),
    CONSTRAINT UQ_EXT_B2B_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_EXT_B2B_Tenant_Enterprise_Code UNIQUE (TenantId, EnterpriseId, ContractCode),
    CONSTRAINT CK_EXT_B2B_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_EXT_B2B_Enterprise FOREIGN KEY (TenantId, EnterpriseId)
        REFERENCES dbo.Enterprise (TenantId, Id),

    CONSTRAINT FK_EXT_B2B_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_EXT_B2B_Tenant_Enterprise
    ON dbo.EXT_EnterpriseB2BContract (TenantId, EnterpriseId)
    WHERE IsDeleted = 0;
GO

/* =======================================================================
   Facility / partner pricing overlays (per module list header)
   ======================================================================= */
CREATE TABLE dbo.EXT_FacilityServicePriceList (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EXT_FSPL_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EXT_FSPL_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_FSPL_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EXT_FSPL_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_FSPL_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EXT_FSPL_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PriceListCode NVARCHAR(80) NOT NULL,
    PriceListName NVARCHAR(250) NOT NULL,
    ServiceModule NVARCHAR(30) NOT NULL,
    PartnerReferenceCode NVARCHAR(80) NULL,
    CurrencyCode NVARCHAR(10) NOT NULL CONSTRAINT DF_EXT_FSPL_Currency DEFAULT ('INR'),
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_EXT_FacilityServicePriceList PRIMARY KEY (Id),
    CONSTRAINT UQ_EXT_FSPL_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_EXT_FSPL_Tenant_Facility_Code UNIQUE (TenantId, FacilityId, PriceListCode),
    CONSTRAINT CK_EXT_FSPL_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_EXT_FSPL_Facility FOREIGN KEY (TenantId, FacilityId)
        REFERENCES dbo.Facility (TenantId, Id)
);
GO

CREATE TABLE dbo.EXT_FacilityServicePriceListLine (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EXT_FSPLL_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EXT_FSPLL_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_FSPLL_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EXT_FSPLL_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_FSPLL_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EXT_FSPLL_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PriceListId BIGINT NOT NULL,
    ServiceItemCode NVARCHAR(80) NOT NULL,
    ServiceItemName NVARCHAR(250) NULL,
    UnitPrice DECIMAL(18,4) NOT NULL,
    TaxCategoryCode NVARCHAR(40) NULL,

    CONSTRAINT PK_EXT_FacilityServicePriceListLine PRIMARY KEY (Id),
    CONSTRAINT UQ_EXT_FSPLL_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_EXT_FSPLL_List_Item UNIQUE (TenantId, FacilityId, PriceListId, ServiceItemCode),

    CONSTRAINT FK_EXT_FSPLL_Header FOREIGN KEY (TenantId, FacilityId, PriceListId)
        REFERENCES dbo.EXT_FacilityServicePriceList (TenantId, FacilityId, Id)
);
GO

/* =======================================================================
   Cross-facility reporting audit (who ran what, scope)
   ======================================================================= */
CREATE TABLE dbo.EXT_CrossFacilityReportAudit (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EXT_CFRA_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EXT_CFRA_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_CFRA_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EXT_CFRA_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_CFRA_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EXT_CFRA_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ReportCode NVARCHAR(80) NOT NULL,
    ReportName NVARCHAR(250) NULL,
    FacilityScopeJson NVARCHAR(MAX) NULL,
    FilterJson NVARCHAR(MAX) NULL,
    ResultRowCount INT NULL,
    CompletedOn DATETIME2(7) NULL,

    CONSTRAINT PK_EXT_CrossFacilityReportAudit PRIMARY KEY (Id),
    CONSTRAINT UQ_EXT_CFRA_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_EXT_CFRA_Tenant_ReportCode
    ON dbo.EXT_CrossFacilityReportAudit (TenantId, ReportCode, CreatedOn DESC)
    WHERE IsDeleted = 0;
GO

/* =======================================================================
   Inter-module integration handoff log (correlation / orchestration)
   ======================================================================= */
CREATE TABLE dbo.EXT_ModuleIntegrationHandoff (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EXT_MIH_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EXT_MIH_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_MIH_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EXT_MIH_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_MIH_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EXT_MIH_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CorrelationId NVARCHAR(64) NOT NULL,
    SourceModule NVARCHAR(30) NOT NULL,
    TargetModule NVARCHAR(30) NOT NULL,
    EntityType NVARCHAR(80) NOT NULL,
    SourceEntityId BIGINT NULL,
    TargetEntityId BIGINT NULL,
    StatusCode NVARCHAR(40) NOT NULL,
    DetailJson NVARCHAR(MAX) NULL,

    CONSTRAINT PK_EXT_ModuleIntegrationHandoff PRIMARY KEY (Id),
    CONSTRAINT UQ_EXT_MIH_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_EXT_MIH_Correlation
    ON dbo.EXT_ModuleIntegrationHandoff (TenantId, CorrelationId)
    WHERE IsDeleted = 0;
GO

/* =======================================================================
   Tenant onboarding checkpoints (multi-tenant provisioning)
   ======================================================================= */
CREATE TABLE dbo.EXT_TenantOnboardingStage (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EXT_TOS_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EXT_TOS_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_TOS_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EXT_TOS_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_TOS_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EXT_TOS_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    StageCode NVARCHAR(80) NOT NULL,
    StageStatus NVARCHAR(40) NOT NULL,
    CompletedOn DATETIME2(7) NULL,
    MetadataJson NVARCHAR(MAX) NULL,

    CONSTRAINT PK_EXT_TenantOnboardingStage PRIMARY KEY (Id),
    CONSTRAINT UQ_EXT_TOS_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_EXT_TOS_Tenant_Stage UNIQUE (TenantId, StageCode)
);
GO

/* =======================================================================
   LIS critical-value escalation trail (logical FKs to LIS in app layer)
   ======================================================================= */
CREATE TABLE dbo.EXT_LabCriticalValueEscalation (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EXT_LCVE_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EXT_LCVE_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_LCVE_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EXT_LCVE_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EXT_LCVE_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EXT_LCVE_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderId BIGINT NULL,
    LabOrderItemId BIGINT NULL,
    LabResultId BIGINT NULL,
    EscalationLevel INT NOT NULL CONSTRAINT DF_EXT_LCVE_Level DEFAULT (1),
    ChannelCode NVARCHAR(40) NOT NULL,
    RecipientSummary NVARCHAR(500) NULL,
    DispatchedOn DATETIME2(7) NULL,
    AcknowledgedOn DATETIME2(7) NULL,
    OutcomeCode NVARCHAR(40) NULL,

    CONSTRAINT PK_EXT_LabCriticalValueEscalation PRIMARY KEY (Id),
    CONSTRAINT UQ_EXT_LCVE_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_EXT_LCVE_Tenant_Facility_Result
    ON dbo.EXT_LabCriticalValueEscalation (TenantId, FacilityId, LabResultId, CreatedOn DESC)
    WHERE IsDeleted = 0 AND LabResultId IS NOT NULL;
GO
