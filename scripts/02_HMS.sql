SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
  HMS (Hospital Management System) - Microservice-ready module tables
  Depends on:
    - Root hierarchy (Facility, Department)
    - Shared domain (Patient, Doctor, Unit, ReferenceDataDefinition/Value)

  Cross-module FK notes:
    - LIS_LabOrder and Pharmacy_PharmacySales are referenced from HMS_BillingItems,
      but their FK constraints are added after those modules are created (via ALTER TABLE)
      to keep module execution order clean.
*/

/* =======================================================================
   HMS VisitType (catalog)
   ======================================================================= */
CREATE TABLE dbo.HMS_VisitType (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_VisitType_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_VisitType_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_VisitType_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_VisitType_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_VisitType_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_VisitType_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    VisitTypeCode NVARCHAR(80) NOT NULL,
    VisitTypeName NVARCHAR(250) NOT NULL,
    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_HMS_VisitType PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_VisitType_Tenant_Code UNIQUE (TenantId, VisitTypeCode),
    CONSTRAINT UQ_HMS_VisitType_TenantId_Id UNIQUE (TenantId, Id),
    CONSTRAINT CK_HMS_VisitType_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom)
);
GO

/* =======================================================================
   Appointment
   ======================================================================= */
CREATE TABLE dbo.HMS_Appointment (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Appointment_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Appointment_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Appointment_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Appointment_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Appointment_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Appointment_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AppointmentNo NVARCHAR(60) NOT NULL,
    PatientId BIGINT NOT NULL,
    DoctorId BIGINT NOT NULL,
    DepartmentId BIGINT NOT NULL,

    VisitTypeId BIGINT NULL,
    AppointmentStatusValueId BIGINT NOT NULL,

    ScheduledStartOn DATETIME2(7) NOT NULL,
    ScheduledEndOn DATETIME2(7) NULL,
    PriorityReferenceValueId BIGINT NULL,
    Reason NVARCHAR(1000) NULL,

    EffectiveFrom DATE NULL,
    EffectiveTo DATE NULL,

    CONSTRAINT PK_HMS_Appointment PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Appointment_Tenant_Facility_No UNIQUE (TenantId, FacilityId, AppointmentNo),
    CONSTRAINT UQ_HMS_Appointment_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),
    CONSTRAINT CK_HMS_Appointment_EffectiveRange CHECK (EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),

    CONSTRAINT FK_HMS_Appointment_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_HMS_Appointment_Doctor FOREIGN KEY (TenantId, DoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_HMS_Appointment_Department FOREIGN KEY (TenantId, DepartmentId)
        REFERENCES dbo.Department(TenantId, Id),
    CONSTRAINT FK_HMS_Appointment_VisitType FOREIGN KEY (TenantId, VisitTypeId)
        REFERENCES dbo.HMS_VisitType(TenantId, Id),
    CONSTRAINT FK_HMS_Appointment_Status FOREIGN KEY (TenantId, AppointmentStatusValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_HMS_Appointment_Priority FOREIGN KEY (TenantId, PriorityReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   Appointment Status History
   ======================================================================= */
CREATE TABLE dbo.HMS_AppointmentStatusHistory (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_AppointmentStatusHistory_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_AppointmentStatusHistory_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_AppointmentStatusHistory_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_AppointmentStatusHistory_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_AppointmentStatusHistory_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_AppointmentStatusHistory_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AppointmentId BIGINT NOT NULL,
    StatusValueId BIGINT NOT NULL,
    StatusOn DATETIME2(7) NOT NULL,
    StatusNote NVARCHAR(1000) NULL,
    ChangedByDoctorId BIGINT NULL,

    CONSTRAINT PK_HMS_AppointmentStatusHistory PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_AppointmentStatusHistory_Tenant_Apt_StatusOn UNIQUE (TenantId, FacilityId, AppointmentId, StatusOn, StatusValueId),

    CONSTRAINT FK_HMS_AppointmentStatusHistory_Appointment FOREIGN KEY (TenantId, FacilityId, AppointmentId)
        REFERENCES dbo.HMS_Appointment(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_AppointmentStatusHistory_Status FOREIGN KEY (TenantId, StatusValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_HMS_AppointmentStatusHistory_ChangedByDoctor FOREIGN KEY (TenantId, ChangedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   Appointment Queue
   ======================================================================= */
CREATE TABLE dbo.HMS_AppointmentQueue (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_AppointmentQueue_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_AppointmentQueue_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_AppointmentQueue_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_AppointmentQueue_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_AppointmentQueue_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_AppointmentQueue_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    AppointmentId BIGINT NOT NULL,
    QueueToken NVARCHAR(60) NOT NULL,
    PositionInQueue INT NOT NULL CONSTRAINT DF_HMS_AppointmentQueue_Position DEFAULT (0),
    QueueStatusReferenceValueId BIGINT NOT NULL,
    EnqueuedOn DATETIME2(7) NOT NULL,
    CheckedInOn DATETIME2(7) NULL,
    ExpectedServiceOn DATETIME2(7) NULL,

    CONSTRAINT PK_HMS_AppointmentQueue PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_AppointmentQueue_Tenant_Facility_Token UNIQUE (TenantId, FacilityId, QueueToken),
    CONSTRAINT UQ_HMS_AppointmentQueue_Tenant_Facility_Apt UNIQUE (TenantId, FacilityId, AppointmentId),

    CONSTRAINT FK_HMS_AppointmentQueue_Appointment FOREIGN KEY (TenantId, FacilityId, AppointmentId)
        REFERENCES dbo.HMS_Appointment(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_AppointmentQueue_Status FOREIGN KEY (TenantId, QueueStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   Visit
   ======================================================================= */
CREATE TABLE dbo.HMS_Visit (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Visit_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Visit_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Visit_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Visit_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Visit_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Visit_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    VisitNo NVARCHAR(60) NOT NULL,
    AppointmentId BIGINT NULL,

    PatientId BIGINT NOT NULL,
    DoctorId BIGINT NOT NULL,
    DepartmentId BIGINT NOT NULL,

    VisitTypeId BIGINT NOT NULL,
    VisitStartOn DATETIME2(7) NOT NULL,
    VisitEndOn DATETIME2(7) NULL,

    ChiefComplaint NVARCHAR(2000) NULL,
    CurrentStatusReferenceValueId BIGINT NOT NULL,

    CONSTRAINT PK_HMS_Visit PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Visit_Tenant_Facility_No UNIQUE (TenantId, FacilityId, VisitNo),
    CONSTRAINT UQ_HMS_Visit_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_Visit_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_HMS_Visit_Doctor FOREIGN KEY (TenantId, DoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_HMS_Visit_Department FOREIGN KEY (TenantId, DepartmentId)
        REFERENCES dbo.Department(TenantId, Id),
    CONSTRAINT FK_HMS_Visit_VisitType FOREIGN KEY (TenantId, VisitTypeId)
        REFERENCES dbo.HMS_VisitType(TenantId, Id),
    CONSTRAINT FK_HMS_Visit_Status FOREIGN KEY (TenantId, CurrentStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),

    CONSTRAINT FK_HMS_Visit_Appointment FOREIGN KEY (TenantId, FacilityId, AppointmentId)
        REFERENCES dbo.HMS_Appointment(TenantId, FacilityId, Id)
);
GO

/* =======================================================================
   Vitals (highly granular)
   ======================================================================= */
CREATE TABLE dbo.HMS_Vitals (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Vitals_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Vitals_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Vitals_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Vitals_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Vitals_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Vitals_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    VisitId BIGINT NOT NULL,
    RecordedOn DATETIME2(7) NOT NULL,

    VitalReferenceValueId BIGINT NOT NULL,  -- BP/Pulse/SpO2/etc.
    ValueNumeric DECIMAL(18,4) NULL,
    ValueNumeric2 DECIMAL(18,4) NULL,      -- e.g., systolic/diastolic
    ValueText NVARCHAR(200) NULL,
    UnitId BIGINT NULL,

    RecordedByDoctorId BIGINT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_HMS_Vitals PRIMARY KEY (Id),
    CONSTRAINT FK_HMS_Vitals_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Vitals_Vital FOREIGN KEY (TenantId, VitalReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_HMS_Vitals_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id),
    CONSTRAINT FK_HMS_Vitals_RecordedBy FOREIGN KEY (TenantId, RecordedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   ClinicalNotes
   ======================================================================= */
CREATE TABLE dbo.HMS_ClinicalNotes (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_ClinicalNotes_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_ClinicalNotes_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_ClinicalNotes_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_ClinicalNotes_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_ClinicalNotes_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_ClinicalNotes_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    VisitId BIGINT NOT NULL,
    NoteTypeReferenceValueId BIGINT NOT NULL,
    EncounterSection NVARCHAR(150) NULL, -- SOAP/Assessment/Plan

    NoteText NVARCHAR(MAX) NOT NULL,
    StructuredPayload NVARCHAR(MAX) NULL,

    AuthorDoctorId BIGINT NULL,

    CONSTRAINT PK_HMS_ClinicalNotes PRIMARY KEY (Id),
    CONSTRAINT FK_HMS_ClinicalNotes_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_ClinicalNotes_NoteType FOREIGN KEY (TenantId, NoteTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_HMS_ClinicalNotes_AuthorDoctor FOREIGN KEY (TenantId, AuthorDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   Diagnosis (ICD mapping support)
   ======================================================================= */
CREATE TABLE dbo.HMS_Diagnosis (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Diagnosis_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Diagnosis_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Diagnosis_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Diagnosis_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Diagnosis_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Diagnosis_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    VisitId BIGINT NOT NULL,
    DiagnosisTypeReferenceValueId BIGINT NULL, -- primary/secondary/etc

    ICDSystem NVARCHAR(30) NOT NULL,
    ICDCode NVARCHAR(30) NOT NULL,
    ICDVersion NVARCHAR(20) NULL,
    ICDDescription NVARCHAR(500) NULL,
    DiagnosisOn DATE NULL,

    CONSTRAINT PK_HMS_Diagnosis PRIMARY KEY (Id),
    CONSTRAINT FK_HMS_Diagnosis_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Diagnosis_DiagType FOREIGN KEY (TenantId, DiagnosisTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   Procedures
   ======================================================================= */
CREATE TABLE dbo.HMS_Procedure (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Procedure_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Procedure_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Procedure_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Procedure_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Procedure_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Procedure_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    VisitId BIGINT NOT NULL,
    ProcedureCode NVARCHAR(50) NOT NULL,
    ProcedureSystem NVARCHAR(30) NULL,
    ProcedureDescription NVARCHAR(500) NULL,
    PerformedOn DATETIME2(7) NULL,
    PerformedByDoctorId BIGINT NULL,
    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_HMS_Procedure PRIMARY KEY (Id),
    CONSTRAINT FK_HMS_Procedure_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Procedure_Doctor FOREIGN KEY (TenantId, PerformedByDoctorId)
        REFERENCES dbo.Doctor(TenantId, Id)
);
GO

/* =======================================================================
   Prescription (header)
   ======================================================================= */
CREATE TABLE dbo.HMS_Prescription (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_Prescription_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_Prescription_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Prescription_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Prescription_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_Prescription_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_Prescription_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PrescriptionNo NVARCHAR(60) NOT NULL,

    VisitId BIGINT NOT NULL,
    PatientId BIGINT NOT NULL,
    DoctorId BIGINT NOT NULL,

    PrescribedOn DATETIME2(7) NOT NULL,
    PrescriptionStatusReferenceValueId BIGINT NOT NULL,

    ValidFrom DATETIME2(7) NULL,
    ValidTo DATETIME2(7) NULL,

    Indication NVARCHAR(1000) NULL,
    Notes NVARCHAR(MAX) NULL,

    CONSTRAINT PK_HMS_Prescription PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_Prescription_Tenant_Facility_No UNIQUE (TenantId, FacilityId, PrescriptionNo),
    CONSTRAINT UQ_HMS_Prescription_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_Prescription_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_Prescription_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_HMS_Prescription_Doctor FOREIGN KEY (TenantId, DoctorId)
        REFERENCES dbo.Doctor(TenantId, Id),
    CONSTRAINT FK_HMS_Prescription_Status FOREIGN KEY (TenantId, PrescriptionStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   PrescriptionItems
   ======================================================================= */
CREATE TABLE dbo.HMS_PrescriptionItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PrescriptionItems_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PrescriptionItems_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PrescriptionItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PrescriptionItems_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PrescriptionItems_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PrescriptionItems_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,
    PrescriptionId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_HMS_PrescriptionItems_Line DEFAULT (0),
    MedicineId BIGINT NOT NULL,         -- FK added later after Pharmacy module is created
    UnitId BIGINT NULL,
    Quantity DECIMAL(18,4) NULL,
    DosageText NVARCHAR(200) NULL,
    FrequencyText NVARCHAR(150) NULL,
    DurationDays INT NULL,
    RouteReferenceValueId BIGINT NULL, -- oral/injection/etc
    IsPRN BIT NOT NULL CONSTRAINT DF_HMS_PrescriptionItems_IsPRN DEFAULT (0),
    ItemNotes NVARCHAR(1000) NULL,

    CONSTRAINT PK_HMS_PrescriptionItems PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_PrescriptionItems_Tenant_Facility UNIQUE (TenantId, FacilityId, PrescriptionId, LineNum),
    CONSTRAINT UQ_HMS_PrescriptionItems_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_PrescriptionItems_Prescription FOREIGN KEY (TenantId, FacilityId, PrescriptionId)
        REFERENCES dbo.HMS_Prescription(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_PrescriptionItems_Unit FOREIGN KEY (TenantId, UnitId)
        REFERENCES dbo.Unit(TenantId, Id),
    CONSTRAINT FK_HMS_PrescriptionItems_Route FOREIGN KEY (TenantId, RouteReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   PrescriptionNotes
   ======================================================================= */
CREATE TABLE dbo.HMS_PrescriptionNotes (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PrescriptionNotes_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PrescriptionNotes_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PrescriptionNotes_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PrescriptionNotes_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PrescriptionNotes_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PrescriptionNotes_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    PrescriptionId BIGINT NOT NULL,
    NoteTypeReferenceValueId BIGINT NOT NULL,
    NoteText NVARCHAR(MAX) NOT NULL,

    CONSTRAINT PK_HMS_PrescriptionNotes PRIMARY KEY (Id),
    CONSTRAINT FK_HMS_PrescriptionNotes_Prescription FOREIGN KEY (TenantId, FacilityId, PrescriptionId)
        REFERENCES dbo.HMS_Prescription(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_PrescriptionNotes_NoteType FOREIGN KEY (TenantId, NoteTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   PaymentModes (catalog for billing)
   ======================================================================= */
CREATE TABLE dbo.HMS_PaymentModes (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PaymentModes_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PaymentModes_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PaymentModes_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PaymentModes_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PaymentModes_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PaymentModes_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    ModeCode NVARCHAR(40) NOT NULL,
    ModeName NVARCHAR(120) NOT NULL,
    SortOrder INT NOT NULL CONSTRAINT DF_HMS_PaymentModes_Sort DEFAULT (0),

    CONSTRAINT PK_HMS_PaymentModes PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_PaymentModes_Tenant_Code UNIQUE (TenantId, ModeCode),
    CONSTRAINT UQ_HMS_PaymentModes_TenantId_Id UNIQUE (TenantId, Id)
);
GO

/* =======================================================================
   BillingHeader
   ======================================================================= */
CREATE TABLE dbo.HMS_BillingHeader (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_BillingHeader_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_BillingHeader_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_BillingHeader_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_BillingHeader_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_BillingHeader_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_BillingHeader_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BillNo NVARCHAR(60) NOT NULL,
    VisitId BIGINT NOT NULL,
    PatientId BIGINT NOT NULL,

    BillDate DATETIME2(7) NOT NULL,
    BillingStatusReferenceValueId BIGINT NOT NULL,

    SubTotal DECIMAL(18,4) NULL,
    TaxTotal DECIMAL(18,4) NULL,
    DiscountTotal DECIMAL(18,4) NULL,
    GrandTotal DECIMAL(18,4) NULL,
    CurrencyCode NVARCHAR(3) NULL,

    Notes NVARCHAR(1000) NULL,

    CONSTRAINT PK_HMS_BillingHeader PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_BillingHeader_Tenant_Facility_No UNIQUE (TenantId, FacilityId, BillNo),
    CONSTRAINT UQ_HMS_BillingHeader_TenantFacility_Id UNIQUE (TenantId, FacilityId, Id),

    CONSTRAINT FK_HMS_BillingHeader_Visit FOREIGN KEY (TenantId, FacilityId, VisitId)
        REFERENCES dbo.HMS_Visit(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_BillingHeader_Patient FOREIGN KEY (TenantId, PatientId)
        REFERENCES dbo.Patient(TenantId, Id),
    CONSTRAINT FK_HMS_BillingHeader_Status FOREIGN KEY (TenantId, BillingStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   BillingItems
   ======================================================================= */
CREATE TABLE dbo.HMS_BillingItems (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_BillingItems_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_BillingItems_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_BillingItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_BillingItems_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_BillingItems_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_BillingItems_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BillingHeaderId BIGINT NOT NULL,
    LineNum INT NOT NULL CONSTRAINT DF_HMS_BillingItems_Line DEFAULT (0),

    ServiceTypeReferenceValueId BIGINT NOT NULL,
    Description NVARCHAR(500) NULL,
    Quantity DECIMAL(18,4) NOT NULL CONSTRAINT DF_HMS_BillingItems_Qty DEFAULT (1),
    UnitPrice DECIMAL(18,4) NULL,
    LineTotal DECIMAL(18,4) NULL,

    -- Cross-module links (FKs added later)
    LabOrderId BIGINT NULL,                 -- => LIS_LabOrder
    PrescriptionId BIGINT NULL,           -- => HMS_Prescription
    PharmacySalesId BIGINT NULL,         -- => Pharmacy_PharmacySales

    ExternalReference NVARCHAR(120) NULL,

    CONSTRAINT PK_HMS_BillingItems PRIMARY KEY (Id),
    CONSTRAINT UQ_HMS_BillingItems_Tenant_Facility_Header_Line UNIQUE (TenantId, FacilityId, BillingHeaderId, LineNum),

    CONSTRAINT FK_HMS_BillingItems_Header FOREIGN KEY (TenantId, FacilityId, BillingHeaderId)
        REFERENCES dbo.HMS_BillingHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_BillingItems_ServiceType FOREIGN KEY (TenantId, ServiceTypeReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id),
    CONSTRAINT FK_HMS_BillingItems_Prescription FOREIGN KEY (TenantId, FacilityId, PrescriptionId)
        REFERENCES dbo.HMS_Prescription(TenantId, FacilityId, Id)
);
GO
-- LabOrderId and PharmacySalesId are cross-module; FK constraints are attached later.

/* =======================================================================
   PaymentTransactions
   ======================================================================= */
CREATE TABLE dbo.HMS_PaymentTransactions (
    Id BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    FacilityId BIGINT NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_HMS_PaymentTransactions_IsActive DEFAULT (1),
    IsDeleted BIT NOT NULL CONSTRAINT DF_HMS_PaymentTransactions_IsDeleted DEFAULT (0),
    CreatedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PaymentTransactions_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PaymentTransactions_CreatedBy DEFAULT (0),
    ModifiedOn DATETIME2(7) NOT NULL CONSTRAINT DF_HMS_PaymentTransactions_ModifiedOn DEFAULT (SYSUTCDATETIME()),
    ModifiedBy BIGINT NOT NULL CONSTRAINT DF_HMS_PaymentTransactions_ModifiedBy DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    BillingHeaderId BIGINT NOT NULL,
    PaymentModeId BIGINT NOT NULL,

    Amount DECIMAL(18,4) NOT NULL,
    TransactionOn DATETIME2(7) NOT NULL,
    TransactionStatusReferenceValueId BIGINT NOT NULL,
    ReferenceNo NVARCHAR(120) NULL,
    Notes NVARCHAR(500) NULL,

    CONSTRAINT PK_HMS_PaymentTransactions PRIMARY KEY (Id),
    CONSTRAINT FK_HMS_PaymentTransactions_Header FOREIGN KEY (TenantId, FacilityId, BillingHeaderId)
        REFERENCES dbo.HMS_BillingHeader(TenantId, FacilityId, Id),
    CONSTRAINT FK_HMS_PaymentTransactions_Mode FOREIGN KEY (TenantId, PaymentModeId)
        REFERENCES dbo.HMS_PaymentModes(TenantId, Id),
    CONSTRAINT FK_HMS_PaymentTransactions_Status FOREIGN KEY (TenantId, TransactionStatusReferenceValueId)
        REFERENCES dbo.ReferenceDataValue(TenantId, Id)
);
GO

/* =======================================================================
   Indexing (HMS hot paths)
   ======================================================================= */
CREATE NONCLUSTERED INDEX IX_HMS_Appointment_Tenant_Facility_StatusTime
ON dbo.HMS_Appointment (TenantId, FacilityId, AppointmentStatusValueId, ScheduledStartOn)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_AppointmentQueue_Tenant_Facility_Status
ON dbo.HMS_AppointmentQueue (TenantId, FacilityId, QueueStatusReferenceValueId, EnqueuedOn)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_Visit_Tenant_Facility_PatientStatusDate
ON dbo.HMS_Visit (TenantId, FacilityId, PatientId, CurrentStatusReferenceValueId, VisitStartOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_Vitals_Tenant_Facility_VisitRecordedOn
ON dbo.HMS_Vitals (TenantId, FacilityId, VisitId, RecordedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_ClinicalNotes_Tenant_Facility_Visit
ON dbo.HMS_ClinicalNotes (TenantId, FacilityId, VisitId, NoteTypeReferenceValueId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_Diagnosis_Tenant_Facility_Visit
ON dbo.HMS_Diagnosis (TenantId, FacilityId, VisitId)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_Prescription_Tenant_Facility_StatusOn
ON dbo.HMS_Prescription (TenantId, FacilityId, PrescriptionStatusReferenceValueId, PrescribedOn DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_PrescriptionItems_Tenant_Facility_Prescription
ON dbo.HMS_PrescriptionItems (TenantId, FacilityId, PrescriptionId, LineNum)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_BillingHeader_Tenant_Facility_StatusDate
ON dbo.HMS_BillingHeader (TenantId, FacilityId, BillingStatusReferenceValueId, BillDate DESC)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_BillingItems_Tenant_Facility_Header
ON dbo.HMS_BillingItems (TenantId, FacilityId, BillingHeaderId, LineNum)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_HMS_PaymentTransactions_Tenant_Facility_HeaderOn
ON dbo.HMS_PaymentTransactions (TenantId, FacilityId, BillingHeaderId, TransactionOn DESC)
WHERE IsDeleted = 0;
GO

/* =======================================================================
   Suggested Partitioning (optional, when data volume is large)
   - Use monthly RANGE partitioning on datetime columns.
   - Partition key examples:
       * HMS_BillingHeader: BillDate
       * HMS_BillingItems: CreatedOn (or via BillingHeaderId mapping)
       * HMS_PaymentTransactions: TransactionOn
       * HMS_Vitals: RecordedOn
       * HMS_AppointmentStatusHistory: StatusOn / CreatedOn
   ======================================================================= */

