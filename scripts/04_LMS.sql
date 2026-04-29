SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  LMS (Lab Management System) - Lab Operations
  Tables:
    - WorkQueue
    - TechnicianAssignment
    - ProcessingStages
    - Equipment
    - EquipmentMaintenance
    - EquipmentCalibration
    - QCRecords
    - QCResults
    - LabInventory
    - LabInventoryTransactions
*/

/* =======================================================================
   ProcessingStages (workflow configuration; facility-bound)
   ======================================================================= */
CREATE TABLE dbo.ProcessingStages (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_ProcessingStages_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_ProcessingStages_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ProcessingStages_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_ProcessingStages_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_ProcessingStages_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_ProcessingStages_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    StageCode NVARCHAR(80) NOT NULL,
    StageName NVARCHAR(250) NOT NULL,
    SequenceNo INT NOT NULL CONSTRAINT DF_ProcessingStages_Seq DEFAULT (0),
    StageNotes NVARCHAR(500) NULL,

    CONSTRAINT PK_ProcessingStages PRIMARY KEY (Id),
    CONSTRAINT UQ_ProcessingStages_TenantFacility_Code UNIQUE (TenantId, FacilityId, StageCode),
    CONSTRAINT UQ_ProcessingStages_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id)
);
GO

/* =======================================================================
   WorkQueue (operational queue entries)
   ======================================================================= */
CREATE TABLE dbo.WorkQueue (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_WorkQueue_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_WorkQueue_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_WorkQueue_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_WorkQueue_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_WorkQueue_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_WorkQueue_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabOrderItemId BIGINT NOT NULL,
    StageId BIGINT NOT NULL,

    PriorityReferenceValueId BIGINT NULL,
    QueueStatusReferenceValueId BIGINT NOT NULL,

    QueuedOn DATETIME2(7) NOT NULL,
    ClaimedOn DATETIME2(7) NULL,
    CompletedOn DATETIME2(7) NULL,

    AssignedByDoctorId BIGINT NULL,
    AssignedTechnicianDoctorId BIGINT NULL,

    QueueNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_WorkQueue PRIMARY KEY (Id),
    CONSTRAINT UQ_WorkQueue_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT UQ_WorkQueue_TenantFacility_OrderItem_Stage UNIQUE (TenantId, FacilityId, LabOrderItemId, StageId),

    CONSTRAINT FK_WorkQueue_OrderItem FOREIGN KEY (TenantId, FacilityId, LabOrderItemId)
        REFERENCES dbo.LIS_LabOrderItems(TenantId, FacilityId, Id),
    CONSTRAINT FK_WorkQueue_Stage FOREIGN KEY (TenantId, FacilityId, StageId)
        REFERENCES dbo.ProcessingStages(TenantId, FacilityId, Id),
    CONSTRAINT FK_WorkQueue_Priority FOREIGN KEY (TenantId, PriorityReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_WorkQueue_Status FOREIGN KEY (TenantId, QueueStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_WorkQueue_AssignedBy FOREIGN KEY (TenantId, AssignedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_WorkQueue_AssignedTech FOREIGN KEY (TenantId, AssignedTechnicianDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   TechnicianAssignment (who claimed work)
   ======================================================================= */
CREATE TABLE dbo.TechnicianAssignment (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_TechnicianAssignment_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_TechnicianAssignment_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_TechnicianAssignment_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_TechnicianAssignment_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_TechnicianAssignment_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_TechnicianAssignment_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    WorkQueueId BIGINT NOT NULL,
    TechnicianDoctorId BIGINT NOT NULL,

    AssignmentStatusReferenceValueId BIGINT NOT NULL,
    AssignedOn DATETIME2(7) NOT NULL,
    ReleasedOn DATETIME2(7) NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_TechnicianAssignment PRIMARY KEY (Id),
    CONSTRAINT UQ_TechnicianAssignment_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_TechnicianAssignment_WorkQueue FOREIGN KEY (TenantId, FacilityId, WorkQueueId)
        REFERENCES dbo.WorkQueue(TenantId, FacilityId, Id),
    CONSTRAINT FK_TechnicianAssignment_Technician FOREIGN KEY (TenantId, TechnicianDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_TechnicianAssignment_Status FOREIGN KEY (TenantId, AssignmentStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   Equipment (facility-bound instrument master)
   ======================================================================= */
CREATE TABLE dbo.Equipment (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Equipment_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_Equipment_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Equipment_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_Equipment_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_Equipment_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_Equipment_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EquipmentCode NVARCHAR(80) NOT NULL,
    EquipmentName NVARCHAR(250) NOT NULL,
    EquipmentTypeReferenceValueId BIGINT NULL,
    SerialNumber NVARCHAR(120) NULL,

    EquipmentNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_Equipment PRIMARY KEY (Id),
    CONSTRAINT UQ_Equipment_TenantFacility_Code UNIQUE (TenantId, FacilityId, EquipmentCode),
    CONSTRAINT UQ_Equipment_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_Equipment_Type FOREIGN KEY (TenantId, EquipmentTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   EquipmentMaintenance
   ======================================================================= */
CREATE TABLE dbo.EquipmentMaintenance (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EquipmentMaintenance_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EquipmentMaintenance_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EquipmentMaintenance_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EquipmentMaintenance_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EquipmentMaintenance_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EquipmentMaintenance_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EquipmentId BIGINT NOT NULL,
    MaintenanceTypeReferenceValueId BIGINT NULL,
    MaintenanceStatusReferenceValueId BIGINT NULL,

    ScheduledOn DATETIME2(7) NULL,
    PerformedOn DATETIME2(7) NULL,
    PerformedByDoctorId BIGINT NULL,

    MaintenanceNotes NVARCHAR(MAX) NULL,
    NextDueOn DATETIME2(7) NULL,

    CONSTRAINT PK_EquipmentMaintenance PRIMARY KEY (Id),
    CONSTRAINT UQ_EquipmentMaintenance_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_EquipmentMaintenance_Equipment FOREIGN KEY (TenantId, FacilityId, EquipmentId)
        REFERENCES dbo.Equipment(TenantId, FacilityId, Id),
    CONSTRAINT FK_EquipmentMaintenance_Type FOREIGN KEY (TenantId, MaintenanceTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_EquipmentMaintenance_Status FOREIGN KEY (TenantId, MaintenanceStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_EquipmentMaintenance_PerformedBy FOREIGN KEY (TenantId, PerformedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   EquipmentCalibration
   ======================================================================= */
CREATE TABLE dbo.EquipmentCalibration (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_EquipmentCalibration_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_EquipmentCalibration_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EquipmentCalibration_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_EquipmentCalibration_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_EquipmentCalibration_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_EquipmentCalibration_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    EquipmentId BIGINT NOT NULL,
    CalibratedOn DATETIME2(7) NOT NULL,
    CalibratorDoctorId BIGINT NULL,

    ResultNumeric DECIMAL(18,4) NULL,
    ResultText NVARCHAR(1000) NULL,
    ValidUntil DATETIME2(7) NULL,
    IsWithinTolerance BIT NOT NULL CONSTRAINT DF_EquipmentCalibration_IsWithinTolerance DEFAULT (1),
    Comments NVARCHAR(MAX) NULL,

    CONSTRAINT PK_EquipmentCalibration PRIMARY KEY (Id),
    CONSTRAINT UQ_EquipmentCalibration_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_EquipmentCalibration_Equipment FOREIGN KEY (TenantId, FacilityId, EquipmentId)
        REFERENCES dbo.Equipment(TenantId, FacilityId, Id),
    CONSTRAINT FK_EquipmentCalibration_Calibrator FOREIGN KEY (TenantId, CalibratorDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   QCRecords
   ======================================================================= */
CREATE TABLE dbo.QCRecords (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_QCRecords_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_QCRecords_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_QCRecords_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_QCRecords_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_QCRecords_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_QCRecords_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    QCTypeReferenceValueId BIGINT NULL,
    QCStatusReferenceValueId BIGINT NOT NULL,

    ScheduledOn DATETIME2(7) NULL,
    PerformedOn DATETIME2(7) NULL,

    PerformedByDoctorId BIGINT NULL,
    LotNo NVARCHAR(120) NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_QCRecords PRIMARY KEY (Id),
    CONSTRAINT UQ_QCRecords_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_QCRecords_QCType FOREIGN KEY (TenantId, QCTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_QCRecords_QCStatus FOREIGN KEY (TenantId, QCStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_QCRecords_PerformedBy FOREIGN KEY (TenantId, PerformedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   QCResults (parameter-wise lines)
   ======================================================================= */
CREATE TABLE dbo.QCResults (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_QCResults_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_QCResults_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_QCResults_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_QCResults_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_QCResults_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_QCResults_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    QCRecordId BIGINT NOT NULL,
    TestParameterId BIGINT NULL,

    ResultNumeric DECIMAL(18,4) NULL,
    ResultText NVARCHAR(MAX) NULL,
    ResultUnitId BIGINT NULL,
    IsPass BIT NOT NULL CONSTRAINT DF_QCResults_IsPass DEFAULT (1),
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_QCResults PRIMARY KEY (Id),
    CONSTRAINT UQ_QCResults_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_QCResults_QCRecord FOREIGN KEY (TenantId, FacilityId, QCRecordId)
        REFERENCES dbo.QCRecords(TenantId, FacilityId, Id),
    CONSTRAINT FK_QCResults_TestParameter FOREIGN KEY (TenantId, TestParameterId)
        REFERENCES dbo.LIS_TestParameters(TenantId, Id),
    CONSTRAINT FK_QCResults_Unit FOREIGN KEY (TenantId, ResultUnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

/* =======================================================================
   LabInventory (reagents/consumables snapshot)
   ======================================================================= */
CREATE TABLE dbo.LabInventory (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LabInventory_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LabInventory_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LabInventory_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LabInventory_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LabInventory_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LabInventory_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    InventoryItemCode NVARCHAR(80) NOT NULL,
    InventoryItemName NVARCHAR(250) NOT NULL,
    UnitId BIGINT NULL,
    CurrentQty DECIMAL(18,4) NOT NULL CONSTRAINT DF_LabInventory_CurrentQty DEFAULT (0),

    CONSTRAINT PK_LabInventory PRIMARY KEY (Id),
    CONSTRAINT UQ_LabInventory_TenantFacility_Code UNIQUE (TenantId, FacilityId, InventoryItemCode),
    CONSTRAINT UQ_LabInventory_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LabInventory_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id)
);
GO

/* =======================================================================
   LabInventoryTransactions (movement ledger)
   ======================================================================= */
CREATE TABLE dbo.LabInventoryTransactions (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LabInventoryTransactions_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_LabInventoryTransactions_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LabInventoryTransactions_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_LabInventoryTransactions_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_LabInventoryTransactions_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_LabInventoryTransactions_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    LabInventoryId BIGINT NOT NULL,
    TransactionTypeReferenceValueId BIGINT NULL,
    QuantityDelta DECIMAL(18,4) NOT NULL,
    TransactionOn DATETIME2(7) NOT NULL,
    PerformedByDoctorId BIGINT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_LabInventoryTransactions PRIMARY KEY (Id),
    CONSTRAINT UQ_LabInventoryTransactions_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_LabInventoryTransactions_Inventory FOREIGN KEY (TenantId, FacilityId, LabInventoryId)
        REFERENCES dbo.LabInventory(TenantId, FacilityId, Id),
    CONSTRAINT FK_LabInventoryTransactions_Type FOREIGN KEY (TenantId, TransactionTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_LabInventoryTransactions_PerformedBy FOREIGN KEY (TenantId, PerformedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   Indexing (LMS hot paths)
   ======================================================================= */
CREATE NONCLUSTERED INDEX IX_WorkQueue_TenantFacility_Stage_Status_QueuedOn
ON dbo.WorkQueue (TenantId, FacilityId, StageId, QueueStatusReferenceValueId, QueuedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_TechnicianAssignment_TenantFacility_Tech_AssignOn
ON dbo.TechnicianAssignment (TenantId, FacilityId, TechnicianDoctorId, AssignmentStatusReferenceValueId, AssignedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_EquipmentMaintenance_TenantFacility_Equipment_On
ON dbo.EquipmentMaintenance (TenantId, FacilityId, EquipmentId, PerformedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_QCResults_TenantFacility_QCRecord
ON dbo.QCResults (TenantId, FacilityId, QCRecordId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_LabInventoryTransactions_TenantFacility_Inventory_On
ON dbo.LabInventoryTransactions (TenantId, FacilityId, LabInventoryId, TransactionOn DESC)
WHERE IsDeleted = 0;
GO

/* =======================================================================
   Suggested Partitioning (optional, for high write workloads)
   - Partition by month on:
       * WorkQueue.QueuedOn
       * QCResults / QCRecords.Could use CreatedOn/PerformedOn
       * LabInventoryTransactions.TransactionOn
   ======================================================================= */

