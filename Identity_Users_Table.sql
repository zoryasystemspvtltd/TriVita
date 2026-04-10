/*
  TriVita — [identity].[Identity_Users] table definition
  Aligned with EF Core model (TriVita.UnifiedDatabase / IdentityService.Infrastructure).

  Run against your TriVita SQL Server database. Creates schema [identity] if missing.
*/

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'identity')
    EXEC(N'CREATE SCHEMA [identity] AUTHORIZATION [dbo];');

IF OBJECT_ID(N'[identity].[Identity_Users]', N'U') IS NOT NULL
BEGIN
    PRINT N'Table [identity].[Identity_Users] already exists.';
END
ELSE
BEGIN
    CREATE TABLE [identity].[Identity_Users] (
        [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
        [Email]        NVARCHAR (320) NOT NULL,
        [PasswordHash] NVARCHAR (500) NOT NULL,
        [TenantId]     BIGINT         NOT NULL,
        [FacilityId]   BIGINT         NULL,
        [Role]         NVARCHAR (128) NOT NULL,
        [IsActive]     BIT            NOT NULL,
        CONSTRAINT [PK_Identity_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Identity_Users_TenantId_Email]
        ON [identity].[Identity_Users] ([TenantId] ASC, [Email] ASC);

    PRINT N'Created [identity].[Identity_Users] and unique index IX_Identity_Users_TenantId_Email.';
END
GO
