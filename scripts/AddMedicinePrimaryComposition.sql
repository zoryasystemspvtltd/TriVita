IF COL_LENGTH(N'dbo.Medicine', N'PrimaryCompositionId') IS NULL
BEGIN
  ALTER TABLE dbo.Medicine ADD PrimaryCompositionId BIGINT NULL;
END
GO
IF NOT EXISTS (
  SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Medicine_Composition_Primary'
)
BEGIN
  ALTER TABLE dbo.Medicine WITH NOCHECK
    ADD CONSTRAINT FK_Medicine_Composition_Primary FOREIGN KEY (PrimaryCompositionId)
    REFERENCES dbo.Composition (Id);
END
GO
