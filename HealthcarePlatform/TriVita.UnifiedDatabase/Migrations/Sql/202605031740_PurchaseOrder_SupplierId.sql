-- Idempotent: PurchaseOrder.SupplierId for PO ↔ supplier filtering (purchase bill, etc.)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.tables t ON c.object_id = t.object_id
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE t.name = N'PurchaseOrder' AND s.name = N'dbo' AND c.name = N'SupplierId'
)
BEGIN
    ALTER TABLE [dbo].[PurchaseOrder] ADD [SupplierId] bigint NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes i
    INNER JOIN sys.tables t ON i.object_id = t.object_id
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE t.name = N'PurchaseOrder' AND s.name = N'dbo' AND i.name = N'IX_PurchaseOrder_SupplierId'
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_PurchaseOrder_SupplierId] ON [dbo].[PurchaseOrder] ([SupplierId]);
END;
GO
