SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  Communication & Notification module (06)
  Depends on:
    - Shared domain: ReferenceDataValue (status/type/priority/channel enums)
    - Enterprise hierarchy: optional logical link via ReferenceId (no hard FK to HMS/LIS/etc.)

  Design:
    - Multi-tenant isolation via TenantId (+ FacilityId where required)
    - All enumerations use ReferenceDataValue (no hardcoded enum tables)
    - No cross-module FKs to Patient/LabOrder/etc. — ReferenceId is opaque
*/

/* =======================================================================
   COM_Notification (intent / event)
   ======================================================================= */
CREATE TABLE dbo.COM_Notification (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_COM_Notification_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_COM_Notification_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_Notification_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_COM_Notification_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_Notification_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_COM_Notification_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EventType NVARCHAR(150) NOT NULL,
    ReferenceId BIGINT NULL,
    -- JSON key-value pairs for template rendering at send-time (no cross-module FKs).
    ContextJson NVARCHAR(MAX) NULL,
    PriorityReferenceValueId BIGINT NOT NULL,
    StatusReferenceValueId BIGINT NOT NULL,

    CONSTRAINT PK_COM_Notification PRIMARY KEY (Id),
    CONSTRAINT UQ_COM_Notification_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_COM_Notification_Priority FOREIGN KEY (TenantId, PriorityReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_COM_Notification_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   COM_NotificationRecipient
   ======================================================================= */
CREATE TABLE dbo.COM_NotificationRecipient (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_COM_NotificationRecipient_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_COM_NotificationRecipient_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationRecipient_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationRecipient_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationRecipient_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationRecipient_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    NotificationId BIGINT NOT NULL,
    RecipientTypeReferenceValueId BIGINT NOT NULL,
    RecipientId BIGINT NULL,
    Email NVARCHAR(300) NULL,
    PhoneNumber NVARCHAR(50) NULL,
    IsPrimary BIT NOT NULL CONSTRAINT DF_COM_NotificationRecipient_IsPrimary DEFAULT (0),

    CONSTRAINT PK_COM_NotificationRecipient PRIMARY KEY (Id),
    CONSTRAINT UQ_COM_NotificationRecipient_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_COM_NotificationRecipient_Notification FOREIGN KEY (TenantId, FacilityId, NotificationId)
        REFERENCES dbo.COM_Notification(TenantId, FacilityId, Id),
    CONSTRAINT FK_COM_NotificationRecipient_Type FOREIGN KEY (TenantId, RecipientTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_COM_NotificationRecipient_Notification
    ON dbo.COM_NotificationRecipient (TenantId, FacilityId, NotificationId);
GO

/* =======================================================================
   COM_NotificationTemplate
   ======================================================================= */
CREATE TABLE dbo.COM_NotificationTemplate (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_COM_NotificationTemplate_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_COM_NotificationTemplate_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationTemplate_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationTemplate_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationTemplate_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationTemplate_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    TemplateCode NVARCHAR(100) NOT NULL,
    TemplateName NVARCHAR(250) NOT NULL,
    ChannelTypeReferenceValueId BIGINT NOT NULL,
    SubjectTemplate NVARCHAR(MAX) NULL,
    BodyTemplate NVARCHAR(MAX) NOT NULL,
    Version INT NOT NULL CONSTRAINT DF_COM_NotificationTemplate_Version DEFAULT (1),

    CONSTRAINT PK_COM_NotificationTemplate PRIMARY KEY (Id),
    CONSTRAINT UQ_COM_NotificationTemplate_Tenant_Facility_Code UNIQUE (TenantId, FacilityId, TemplateCode),
    CONSTRAINT UQ_COM_NotificationTemplate_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_COM_NotificationTemplate_ChannelType FOREIGN KEY (TenantId, ChannelTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   COM_NotificationChannel (execution / delivery attempt)
   ======================================================================= */
CREATE TABLE dbo.COM_NotificationChannel (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_COM_NotificationChannel_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_COM_NotificationChannel_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationChannel_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationChannel_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationChannel_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationChannel_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    NotificationId BIGINT NOT NULL,
    ChannelTypeReferenceValueId BIGINT NOT NULL,
    TemplateId BIGINT NULL,
    StatusReferenceValueId BIGINT NOT NULL,
    AttemptCount INT NOT NULL CONSTRAINT DF_COM_NotificationChannel_AttemptCount DEFAULT (0),
    LastAttemptOn DATETIME2(7) NULL,
    SentOn DATETIME2(7) NULL,
    ErrorMessage NVARCHAR(1000) NULL,

    CONSTRAINT PK_COM_NotificationChannel PRIMARY KEY (Id),
    CONSTRAINT UQ_COM_NotificationChannel_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_COM_NotificationChannel_Notification FOREIGN KEY (TenantId, FacilityId, NotificationId)
        REFERENCES dbo.COM_Notification(TenantId, FacilityId, Id),
    CONSTRAINT FK_COM_NotificationChannel_ChannelType FOREIGN KEY (TenantId, ChannelTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_COM_NotificationChannel_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_COM_NotificationChannel_Template FOREIGN KEY (TenantId, FacilityId, TemplateId)
        REFERENCES dbo.COM_NotificationTemplate(TenantId, FacilityId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_COM_NotificationChannel_Notification
    ON dbo.COM_NotificationChannel (TenantId, FacilityId, NotificationId);
GO

/* =======================================================================
   COM_NotificationLog
   ======================================================================= */
CREATE TABLE dbo.COM_NotificationLog (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_COM_NotificationLog_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_COM_NotificationLog_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationLog_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationLog_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationLog_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationLog_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    NotificationChannelId BIGINT NOT NULL,
    AttemptNo INT NOT NULL,
    RequestPayload NVARCHAR(MAX) NULL,
    ResponsePayload NVARCHAR(MAX) NULL,
    StatusReferenceValueId BIGINT NOT NULL,

    CONSTRAINT PK_COM_NotificationLog PRIMARY KEY (Id),
    CONSTRAINT UQ_COM_NotificationLog_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_COM_NotificationLog_Channel FOREIGN KEY (TenantId, FacilityId, NotificationChannelId)
        REFERENCES dbo.COM_NotificationChannel(TenantId, FacilityId, Id),
    CONSTRAINT FK_COM_NotificationLog_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_COM_NotificationLog_Channel
    ON dbo.COM_NotificationLog (TenantId, FacilityId, NotificationChannelId);
GO

/* =======================================================================
   COM_NotificationQueue
   ======================================================================= */
CREATE TABLE dbo.COM_NotificationQueue (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_COM_NotificationQueue_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_COM_NotificationQueue_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationQueue_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationQueue_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_COM_NotificationQueue_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_COM_NotificationQueue_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    NotificationId BIGINT NOT NULL,
    ScheduledOn DATETIME2(7) NOT NULL,
    ProcessedOn DATETIME2(7) NULL,
    StatusReferenceValueId BIGINT NOT NULL,

    CONSTRAINT PK_COM_NotificationQueue PRIMARY KEY (Id),
    CONSTRAINT UQ_COM_NotificationQueue_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_COM_NotificationQueue_Notification FOREIGN KEY (TenantId, FacilityId, NotificationId)
        REFERENCES dbo.COM_Notification(TenantId, FacilityId, Id),
    CONSTRAINT FK_COM_NotificationQueue_Status FOREIGN KEY (TenantId, StatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

CREATE NONCLUSTERED INDEX IX_COM_NotificationQueue_Pending
    ON dbo.COM_NotificationQueue (TenantId, FacilityId, StatusReferenceValueId, ScheduledOn)
    INCLUDE (NotificationId);
GO
