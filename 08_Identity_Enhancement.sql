SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  TriVita — Identity & IAM extensions (08)
  Run after: 00–07 (including 07_LMS_LIS_Enhancement.sql when IAM tables are used).

  Rules:
    - Does NOT ALTER or DROP existing tables from 00–07 or Identity_Users.
    - Adds NEW tables only (Identity_* for JWT/IdentityService store; IAM_* extensions for enterprise IAM from 07).
    - Multi-tenant: TenantId where applicable; FacilityId on tables that are facility-scoped.
    - Status/type enumerations use ReferenceDataValueId where noted.
*/

/* =============================================================================
   -- A) Identity_* — JWT identity store (pairs with EF Core Identity_Users)
   -- Prerequisite: dbo.Identity_Users exists (created by IdentityService EnsureCreated/migrations).
   ============================================================================= */

IF OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.Identity_UserProfile', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Identity_UserProfile (
            UserId BIGINT NOT NULL,
            TenantId BIGINT NOT NULL,
            FacilityId BIGINT NULL,
            IsActive BIT NOT NULL CONSTRAINT DF_Identity_UserProfile_IsActive DEFAULT (1),
            IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_UserProfile_IsDeleted DEFAULT (0),
            CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_UserProfile_CreatedOn DEFAULT (SYSUTCDATETIME()),
            CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_UserProfile_CreatedBy DEFAULT (0),
            ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_UserProfile_ModifiedOn DEFAULT (SYSUTCDATETIME()),
            ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_UserProfile_ModifiedBy DEFAULT (0),
            RowVersion ROWVERSION NOT NULL,

            DisplayName NVARCHAR(250) NULL,
            Phone NVARCHAR(50) NULL,
            PreferredLocaleReferenceValueId BIGINT NULL,
            TimeZoneId NVARCHAR(100) NULL,
            AvatarUrl NVARCHAR(500) NULL,
            MfaEnabled BIT NOT NULL CONSTRAINT DF_Identity_UserProfile_MfaEnabled DEFAULT (0),

            CONSTRAINT PK_Identity_UserProfile PRIMARY KEY (UserId),
            CONSTRAINT FK_Identity_UserProfile_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id)
        );
    END
END
GO

IF OBJECT_ID(N'dbo.Identity_Role', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_Role (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_Role_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_Role_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_Role_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_Role_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_Role_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_Role_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        RoleCode NVARCHAR(80) NOT NULL,
        RoleName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsSystemRole BIT NOT NULL CONSTRAINT DF_Identity_Role_IsSystem DEFAULT (0),

        CONSTRAINT PK_Identity_Role PRIMARY KEY (Id),
        CONSTRAINT UQ_Identity_Role_TenantId_Id UNIQUE (TenantId, Id),
        CONSTRAINT UQ_Identity_Role_Tenant_Code UNIQUE (TenantId, RoleCode)
    );
END
GO

IF OBJECT_ID(N'dbo.Identity_Permission', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_Permission (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_Permission_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_Permission_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_Permission_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_Permission_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_Permission_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_Permission_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        PermissionCode NVARCHAR(120) NOT NULL,
        PermissionName NVARCHAR(250) NOT NULL,
        ModuleCode NVARCHAR(80) NULL,
        Description NVARCHAR(500) NULL,

        CONSTRAINT PK_Identity_Permission PRIMARY KEY (Id),
        CONSTRAINT UQ_Identity_Permission_TenantId_Id UNIQUE (TenantId, Id),
        CONSTRAINT UQ_Identity_Permission_Tenant_Code UNIQUE (TenantId, PermissionCode)
    );
END
GO

IF OBJECT_ID(N'dbo.Identity_UserRole', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_UserRole (
        UserId BIGINT NOT NULL,
        RoleId BIGINT NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_UserRole_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_UserRole_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_UserRole_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_UserRole_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_UserRole_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_UserRole_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        CONSTRAINT PK_Identity_UserRole PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_Identity_UserRole_Role FOREIGN KEY (TenantId, RoleId)
            REFERENCES dbo.Identity_Role(TenantId, Id)
    );
END
GO

IF OBJECT_ID(N'dbo.Identity_UserRole', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Identity_UserRole_User')
BEGIN
    ALTER TABLE dbo.Identity_UserRole
        ADD CONSTRAINT FK_Identity_UserRole_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id);
END
GO

IF OBJECT_ID(N'dbo.Identity_RolePermission', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_RolePermission (
        RoleId BIGINT NOT NULL,
        PermissionId BIGINT NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_RolePermission_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_RolePermission_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_RolePermission_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_RolePermission_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_RolePermission_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_RolePermission_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        CONSTRAINT PK_Identity_RolePermission PRIMARY KEY (RoleId, PermissionId),
        CONSTRAINT FK_Identity_RolePermission_Role FOREIGN KEY (TenantId, RoleId)
            REFERENCES dbo.Identity_Role(TenantId, Id),
        CONSTRAINT FK_Identity_RolePermission_Permission FOREIGN KEY (TenantId, PermissionId)
            REFERENCES dbo.Identity_Permission(TenantId, Id)
    );
END
GO

IF OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Identity_UserFacilityGrant', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_UserFacilityGrant (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_UserFacilityGrant_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_UserFacilityGrant_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_UserFacilityGrant_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_UserFacilityGrant_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_UserFacilityGrant_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_UserFacilityGrant_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NOT NULL,
        GrantFacilityId BIGINT NOT NULL,

        CONSTRAINT PK_Identity_UserFacilityGrant PRIMARY KEY (Id),
        CONSTRAINT UQ_Identity_UserFacilityGrant_User_Grant UNIQUE (UserId, GrantFacilityId),
        CONSTRAINT FK_Identity_UserFacilityGrant_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Identity_RefreshToken', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_RefreshToken (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_RefreshToken_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_RefreshToken_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_RefreshToken_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_RefreshToken_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_RefreshToken_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_RefreshToken_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NOT NULL,
        TokenFamilyId UNIQUEIDENTIFIER NOT NULL,
        TokenHash NVARCHAR(256) NOT NULL,
        ExpiresOn DATETIME2(7) NOT NULL,
        RevokedOn DATETIME2(7) NULL,
        ReplacedByTokenId BIGINT NULL,
        ClientIp NVARCHAR(64) NULL,
        UserAgent NVARCHAR(512) NULL,

        CONSTRAINT PK_Identity_RefreshToken PRIMARY KEY (Id),
        CONSTRAINT FK_Identity_RefreshToken_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id)
    );
    CREATE NONCLUSTERED INDEX IX_Identity_RefreshToken_User_Family
        ON dbo.Identity_RefreshToken (UserId, TokenFamilyId) WHERE IsDeleted = 0;
    CREATE NONCLUSTERED INDEX IX_Identity_RefreshToken_Hash
        ON dbo.Identity_RefreshToken (TokenHash) WHERE IsDeleted = 0 AND RevokedOn IS NULL;
END
GO

IF OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Identity_AccountLockoutState', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_AccountLockoutState (
        UserId BIGINT NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_AccountLockoutState_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_AccountLockoutState_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_AccountLockoutState_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_AccountLockoutState_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_AccountLockoutState_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_AccountLockoutState_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        FailedAttemptCount INT NOT NULL CONSTRAINT DF_Identity_AccountLockoutState_Failed DEFAULT (0),
        LockoutEndOn DATETIME2(7) NULL,
        LastFailedAttemptOn DATETIME2(7) NULL,

        CONSTRAINT PK_Identity_AccountLockoutState PRIMARY KEY (UserId),
        CONSTRAINT FK_Identity_AccountLockoutState_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Identity_LoginAttempt', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_LoginAttempt (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_LoginAttempt_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_LoginAttempt_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_LoginAttempt_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_LoginAttempt_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_LoginAttempt_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_LoginAttempt_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NULL,
        EmailAttempted NVARCHAR(320) NOT NULL,
        Success BIT NOT NULL,
        FailureReasonReferenceValueId BIGINT NULL,
        IpAddress NVARCHAR(64) NULL,
        UserAgent NVARCHAR(512) NULL,
        CorrelationId UNIQUEIDENTIFIER NULL,

        CONSTRAINT PK_Identity_LoginAttempt PRIMARY KEY (Id),
        CONSTRAINT FK_Identity_LoginAttempt_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id)
    );
    CREATE NONCLUSTERED INDEX IX_Identity_LoginAttempt_Tenant_Email_On
        ON dbo.Identity_LoginAttempt (TenantId, EmailAttempted, CreatedOn DESC);
END
GO

IF OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Identity_PasswordResetToken', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_PasswordResetToken (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_PasswordResetToken_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_PasswordResetToken_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_PasswordResetToken_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_PasswordResetToken_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_PasswordResetToken_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_PasswordResetToken_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NOT NULL,
        TokenHash NVARCHAR(256) NOT NULL,
        ExpiresOn DATETIME2(7) NOT NULL,
        ConsumedOn DATETIME2(7) NULL,
        RequestChannelReferenceValueId BIGINT NULL,

        CONSTRAINT PK_Identity_PasswordResetToken PRIMARY KEY (Id),
        CONSTRAINT FK_Identity_PasswordResetToken_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id)
    );
    CREATE NONCLUSTERED INDEX IX_Identity_PasswordResetToken_User_Expires
        ON dbo.Identity_PasswordResetToken (UserId, ExpiresOn DESC) WHERE IsDeleted = 0 AND ConsumedOn IS NULL;
END
GO

IF OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Identity_UserActivityLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_UserActivityLog (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_UserActivityLog_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_UserActivityLog_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_UserActivityLog_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_UserActivityLog_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_UserActivityLog_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_UserActivityLog_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NOT NULL,
        ActivityTypeReferenceValueId BIGINT NOT NULL,
        DetailJson NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(64) NULL,

        CONSTRAINT PK_Identity_UserActivityLog PRIMARY KEY (Id),
        CONSTRAINT FK_Identity_UserActivityLog_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id)
    );
    CREATE NONCLUSTERED INDEX IX_Identity_UserActivityLog_User_On
        ON dbo.Identity_UserActivityLog (UserId, CreatedOn DESC);
END
GO

IF OBJECT_ID(N'dbo.Identity_Users', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.Identity_MfaPendingChallenge', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Identity_MfaPendingChallenge (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Identity_MfaPendingChallenge_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_Identity_MfaPendingChallenge_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_MfaPendingChallenge_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_Identity_MfaPendingChallenge_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Identity_MfaPendingChallenge_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Identity_MfaPendingChallenge_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NOT NULL,
        ChallengeTokenHash NVARCHAR(256) NOT NULL,
        ExpiresOn DATETIME2(7) NOT NULL,
        ConsumedOn DATETIME2(7) NULL,

        CONSTRAINT PK_Identity_MfaPendingChallenge PRIMARY KEY (Id),
        CONSTRAINT FK_Identity_MfaPendingChallenge_User FOREIGN KEY (UserId) REFERENCES dbo.Identity_Users(Id)
    );
END
GO

/* =============================================================================
   -- B) IAM_* extensions (enterprise IAM from script 07)
   ============================================================================= */

IF OBJECT_ID(N'dbo.IAM_UserAccount', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_AuthRefreshToken', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IAM_AuthRefreshToken (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_IAM_AuthRefreshToken_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_AuthRefreshToken_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_AuthRefreshToken_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_AuthRefreshToken_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_AuthRefreshToken_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_AuthRefreshToken_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NOT NULL,
        TokenFamilyId UNIQUEIDENTIFIER NOT NULL,
        TokenHash NVARCHAR(256) NOT NULL,
        ExpiresOn DATETIME2(7) NOT NULL,
        RevokedOn DATETIME2(7) NULL,
        ReplacedByTokenId BIGINT NULL,
        ClientIp NVARCHAR(64) NULL,
        UserAgent NVARCHAR(512) NULL,

        CONSTRAINT PK_IAM_AuthRefreshToken PRIMARY KEY (Id),
        CONSTRAINT UQ_IAM_AuthRefreshToken_TenantId_Id UNIQUE (TenantId, Id),
        CONSTRAINT FK_IAM_AuthRefreshToken_User FOREIGN KEY (TenantId, UserId)
            REFERENCES dbo.IAM_UserAccount(TenantId, Id)
    );
    CREATE NONCLUSTERED INDEX IX_IAM_AuthRefreshToken_User_Family
        ON dbo.IAM_AuthRefreshToken (TenantId, UserId, TokenFamilyId) WHERE IsDeleted = 0;
END
GO

IF OBJECT_ID(N'dbo.IAM_UserAccount', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_UserSecurityState', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IAM_UserSecurityState (
        UserId BIGINT NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_IAM_UserSecurityState_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_UserSecurityState_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserSecurityState_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserSecurityState_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserSecurityState_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserSecurityState_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        FailedLoginCount INT NOT NULL CONSTRAINT DF_IAM_UserSecurityState_Failed DEFAULT (0),
        LockoutEndOn DATETIME2(7) NULL,
        LastFailedLoginOn DATETIME2(7) NULL,

        CONSTRAINT PK_IAM_UserSecurityState PRIMARY KEY (TenantId, UserId),
        CONSTRAINT FK_IAM_UserSecurityState_User FOREIGN KEY (TenantId, UserId)
            REFERENCES dbo.IAM_UserAccount(TenantId, Id)
    );
END
GO

IF OBJECT_ID(N'dbo.IAM_Role', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_RoleHierarchy', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IAM_RoleHierarchy (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_IAM_RoleHierarchy_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_RoleHierarchy_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_RoleHierarchy_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_RoleHierarchy_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_RoleHierarchy_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_RoleHierarchy_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        ParentRoleId BIGINT NOT NULL,
        ChildRoleId BIGINT NOT NULL,

        CONSTRAINT PK_IAM_RoleHierarchy PRIMARY KEY (Id),
        CONSTRAINT UQ_IAM_RoleHierarchy_Tenant_Parent_Child UNIQUE (TenantId, ParentRoleId, ChildRoleId),
        CONSTRAINT UQ_IAM_RoleHierarchy_TenantId_Id UNIQUE (TenantId, Id),
        CONSTRAINT FK_IAM_RoleHierarchy_Parent FOREIGN KEY (TenantId, ParentRoleId)
            REFERENCES dbo.IAM_Role(TenantId, Id),
        CONSTRAINT FK_IAM_RoleHierarchy_Child FOREIGN KEY (TenantId, ChildRoleId)
            REFERENCES dbo.IAM_Role(TenantId, Id),
        CONSTRAINT CK_IAM_RoleHierarchy_NoSelfParent CHECK (ParentRoleId <> ChildRoleId)
    );
END
GO

IF OBJECT_ID(N'dbo.IAM_Permission', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_UserPermissionGrant', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IAM_UserPermissionGrant (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_IAM_UserPermissionGrant_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_UserPermissionGrant_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserPermissionGrant_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserPermissionGrant_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_UserPermissionGrant_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_UserPermissionGrant_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NOT NULL,
        PermissionId BIGINT NOT NULL,
        GrantTypeReferenceValueId BIGINT NOT NULL,
        ResourceTypeReferenceValueId BIGINT NULL,
        ResourceKey NVARCHAR(200) NULL,
        EffectiveFrom DATE NULL,
        EffectiveTo DATE NULL,

        CONSTRAINT PK_IAM_UserPermissionGrant PRIMARY KEY (Id),
        CONSTRAINT UQ_IAM_UserPermissionGrant_TenantId_Id UNIQUE (TenantId, Id),
        CONSTRAINT CK_IAM_UserPermissionGrant_Effective CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),
        CONSTRAINT FK_IAM_UserPermissionGrant_User FOREIGN KEY (TenantId, UserId)
            REFERENCES dbo.IAM_UserAccount(TenantId, Id),
        CONSTRAINT FK_IAM_UserPermissionGrant_Permission FOREIGN KEY (TenantId, PermissionId)
            REFERENCES dbo.IAM_Permission(TenantId, Id),
        CONSTRAINT FK_IAM_UserPermissionGrant_GrantType FOREIGN KEY (TenantId, GrantTypeReferenceValueId)
            REFERENCES dbo.ReferenceDataValue(TenantId, Id),
        CONSTRAINT FK_IAM_UserPermissionGrant_ResType FOREIGN KEY (TenantId, ResourceTypeReferenceValueId)
            REFERENCES dbo.ReferenceDataValue(TenantId, Id)
    );
END
GO

IF OBJECT_ID(N'dbo.IAM_UserAccount', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_LoginAudit', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IAM_LoginAudit (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_IAM_LoginAudit_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_LoginAudit_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_LoginAudit_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_LoginAudit_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_LoginAudit_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_LoginAudit_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        UserId BIGINT NULL,
        LoginNameAttempted NVARCHAR(120) NOT NULL,
        Success BIT NOT NULL,
        FailureReasonReferenceValueId BIGINT NULL,
        IpAddress NVARCHAR(64) NULL,
        UserAgent NVARCHAR(512) NULL,
        CorrelationId UNIQUEIDENTIFIER NULL,

        CONSTRAINT PK_IAM_LoginAudit PRIMARY KEY (Id),
        CONSTRAINT UQ_IAM_LoginAudit_TenantId_Id UNIQUE (TenantId, Id),
        CONSTRAINT FK_IAM_LoginAudit_User FOREIGN KEY (TenantId, UserId)
            REFERENCES dbo.IAM_UserAccount(TenantId, Id),
        CONSTRAINT FK_IAM_LoginAudit_FailReason FOREIGN KEY (TenantId, FailureReasonReferenceValueId)
            REFERENCES dbo.ReferenceDataValue(TenantId, Id)
    );
    CREATE NONCLUSTERED INDEX IX_IAM_LoginAudit_Tenant_Login_On
        ON dbo.IAM_LoginAudit (TenantId, LoginNameAttempted, CreatedOn DESC);
END
GO

IF OBJECT_ID(N'dbo.IAM_Role', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_AuthorizationPolicy', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IAM_AuthorizationPolicy (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_IAM_AuthorizationPolicy_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_AuthorizationPolicy_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_AuthorizationPolicy_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_AuthorizationPolicy_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_AuthorizationPolicy_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_AuthorizationPolicy_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        PolicyCode NVARCHAR(120) NOT NULL,
        PolicyName NVARCHAR(250) NOT NULL,
        DefinitionJson NVARCHAR(MAX) NOT NULL,

        CONSTRAINT PK_IAM_AuthorizationPolicy PRIMARY KEY (Id),
        CONSTRAINT UQ_IAM_AuthorizationPolicy_Tenant_Code UNIQUE (TenantId, PolicyCode),
        CONSTRAINT UQ_IAM_AuthorizationPolicy_TenantId_Id UNIQUE (TenantId, Id)
    );
END
GO

IF OBJECT_ID(N'dbo.IAM_Role', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_RolePolicyBinding', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IAM_RolePolicyBinding (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_IAM_RolePolicyBinding_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_RolePolicyBinding_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_RolePolicyBinding_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_RolePolicyBinding_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_RolePolicyBinding_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_RolePolicyBinding_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        RoleId BIGINT NOT NULL,
        AuthorizationPolicyId BIGINT NOT NULL,

        CONSTRAINT PK_IAM_RolePolicyBinding PRIMARY KEY (Id),
        CONSTRAINT UQ_IAM_RolePolicyBinding_Tenant_Role_Policy UNIQUE (TenantId, RoleId, AuthorizationPolicyId),
        CONSTRAINT UQ_IAM_RolePolicyBinding_TenantId_Id UNIQUE (TenantId, Id),
        CONSTRAINT FK_IAM_RolePolicyBinding_Role FOREIGN KEY (TenantId, RoleId)
            REFERENCES dbo.IAM_Role(TenantId, Id),
        CONSTRAINT FK_IAM_RolePolicyBinding_Policy FOREIGN KEY (TenantId, AuthorizationPolicyId)
            REFERENCES dbo.IAM_AuthorizationPolicy(TenantId, Id)
    );
END
GO

IF OBJECT_ID(N'dbo.BusinessUnit', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_Role', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.IAM_RoleBusinessUnitScope', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IAM_RoleBusinessUnitScope (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        TenantId BIGINT NOT NULL,
        FacilityId BIGINT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_IAM_RoleBusinessUnitScope_IsActive DEFAULT (1),
        IsDeleted BIT NOT NULL CONSTRAINT DF_IAM_RoleBusinessUnitScope_IsDeleted DEFAULT (0),
        CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_RoleBusinessUnitScope_CreatedOn DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NOT NULL CONSTRAINT DF_IAM_RoleBusinessUnitScope_CreatedBy DEFAULT (0),
        ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_IAM_RoleBusinessUnitScope_ModifiedOn DEFAULT (SYSUTCDATETIME()),
        ModifiedBy BIGINT NOT NULL CONSTRAINT DF_IAM_RoleBusinessUnitScope_ModifiedBy DEFAULT (0),
        RowVersion ROWVERSION NOT NULL,

        RoleId BIGINT NOT NULL,
        BusinessUnitId BIGINT NOT NULL,

        CONSTRAINT PK_IAM_RoleBusinessUnitScope PRIMARY KEY (Id),
        CONSTRAINT UQ_IAM_RoleBusinessUnitScope_Tenant_Role_BU UNIQUE (TenantId, RoleId, BusinessUnitId),
        CONSTRAINT UQ_IAM_RoleBusinessUnitScope_TenantId_Id UNIQUE (TenantId, Id),
        CONSTRAINT FK_IAM_RoleBusinessUnitScope_Role FOREIGN KEY (TenantId, RoleId)
            REFERENCES dbo.IAM_Role(TenantId, Id),
        CONSTRAINT FK_IAM_RoleBusinessUnitScope_BU FOREIGN KEY (TenantId, BusinessUnitId)
            REFERENCES dbo.BusinessUnit(TenantId, Id)
    );
END
GO
