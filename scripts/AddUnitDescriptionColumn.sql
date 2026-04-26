-- Run on existing databases created before Description was added to dbo.Unit.
IF COL_LENGTH(N'dbo.Unit', N'Description') IS NULL
BEGIN
    ALTER TABLE dbo.Unit ADD Description NVARCHAR(500) NULL;
END
GO
