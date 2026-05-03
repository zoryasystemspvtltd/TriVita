-- Stock ledger reporting: filter columns + indexes.
-- Target the StockLedger table (dbo or pharmacy schema). Adjust object names if your DB uses schemas.
-- Safe to run multiple times.

IF COL_LENGTH('StockLedger', 'GrnSupplierId') IS NULL
    ALTER TABLE StockLedger ADD GrnSupplierId BIGINT NULL;

IF COL_LENGTH('StockLedger', 'SalePatientId') IS NULL
    ALTER TABLE StockLedger ADD SalePatientId BIGINT NULL;

IF COL_LENGTH('StockLedger', 'SaleCustomerId') IS NULL
    ALTER TABLE StockLedger ADD SaleCustomerId BIGINT NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_StockLedger_TenantId_FacilityId_GrnSupplierId'
      AND object_id = OBJECT_ID('StockLedger'))
    CREATE NONCLUSTERED INDEX IX_StockLedger_TenantId_FacilityId_GrnSupplierId
    ON StockLedger (TenantId, FacilityId, GrnSupplierId);

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_StockLedger_TenantId_FacilityId_MedicineBatchId_TransactionDate'
      AND object_id = OBJECT_ID('StockLedger'))
    CREATE NONCLUSTERED INDEX IX_StockLedger_TenantId_FacilityId_MedicineBatchId_TransactionDate
    ON StockLedger (TenantId, FacilityId, MedicineBatchId, TransactionDate);

-- Backfill from existing documents (ledger remains source of truth for quantities).
UPDATE sl
SET GrnSupplierId = gr.SupplierId,
    SourceReference = COALESCE(NULLIF(LTRIM(RTRIM(sl.SourceReference)), ''), gr.GoodsReceiptNo)
FROM StockLedger AS sl
INNER JOIN GoodsReceipt AS gr ON gr.Id = sl.ReferenceId
WHERE sl.TransactionType = 1
  AND sl.IsDeleted = 0;

UPDATE sl
SET SalePatientId = ps.PatientId,
    SourceReference = COALESCE(NULLIF(LTRIM(RTRIM(sl.SourceReference)), ''), ps.SalesNo)
FROM StockLedger AS sl
INNER JOIN PharmacySalesItems AS psi ON psi.Id = sl.ReferenceLineId
INNER JOIN PharmacySales AS ps ON ps.Id = psi.PharmacySalesId
WHERE sl.TransactionType = 2
  AND sl.IsDeleted = 0;

UPDATE sl
SET SalePatientId = sb.PatientId,
    SaleCustomerId = sb.CustomerId,
    SourceReference = COALESCE(NULLIF(LTRIM(RTRIM(sl.SourceReference)), ''), sb.BillNo)
FROM StockLedger AS sl
INNER JOIN SalesBillItems AS sbi ON sbi.Id = sl.ReferenceLineId
INNER JOIN SalesBill AS sb ON sb.Id = sbi.SalesBillId
WHERE sl.TransactionType = 2
  AND sl.IsDeleted = 0
  AND sl.SalePatientId IS NULL
  AND sl.SaleCustomerId IS NULL;

UPDATE sl
SET SourceReference = COALESCE(NULLIF(LTRIM(RTRIM(sl.SourceReference)), ''), sa.AdjustmentNo)
FROM StockLedger AS sl
INNER JOIN StockAdjustment AS sa ON sa.Id = sl.ReferenceId
WHERE sl.TransactionType = 3
  AND sl.IsDeleted = 0;
