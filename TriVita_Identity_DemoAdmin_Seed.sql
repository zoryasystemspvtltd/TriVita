/*
  TriVita — seed demo Identity RBAC + admin user (schema: identity)

  Target: SQL Server database created by TriVita unified EF migrations (tables under [identity]).
  Run once after migrations. Safe to re-run: skips if demo user already exists.

  Login (portal):
    Email:     admin@demo.local
    Password:  ChangeMe!123
    Tenant ID: 1

  Change database name below if yours differs (e.g. TriVitaHealthcare).
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE [TriVita];
GO

DECLARE @TenantId BIGINT = 1;
DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @DemoEmail NVARCHAR(320) = N'admin@demo.local';
/* Password hash: ASP.NET Core 8 PasswordHasher<AppUser> for "ChangeMe!123" (same as IdentityDataSeeder). */
DECLARE @PasswordHash NVARCHAR(500) = N'AQAAAAIAAYagAAAAEK5IfpOm6uNcEg/mev5DhO5WkIYRf54VXzeJYuYxQXBBjYWSv+BjEbIW6TixxYFJBw==';

IF EXISTS (
    SELECT 1
    FROM [identity].[Identity_Users]
    WHERE [TenantId] = @TenantId AND [Email] = @DemoEmail)
BEGIN
    PRINT N'Demo user already present: ' + @DemoEmail;
    RETURN;
END;

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM [identity].[Identity_Permission] WHERE [TenantId] = @TenantId)
BEGIN
    INSERT INTO [identity].[Identity_Permission] (
        [TenantId], [FacilityId], [IsActive], [IsDeleted],
        [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy],
        [PermissionCode], [PermissionName], [ModuleCode], [Description])
    VALUES
        (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'hms.api',       N'HMS API',                 N'core', NULL),
        (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'lis.api',       N'LIS API',                 N'core', NULL),
        (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'lms.api',       N'LMS API',                 N'core', NULL),
        (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'pharmacy.api',  N'Pharmacy API',            N'core', NULL),
        (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'shared.api',    N'Shared API',              N'core', NULL),
        (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'communication.api', N'Communication API',     N'core', NULL),
        (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'identity.admin', N'Identity administration', N'core', NULL),
        (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'*',            N'Full access',             N'core', NULL);
END;

DECLARE @RoleId BIGINT =
    (SELECT TOP (1) [Id] FROM [identity].[Identity_Role] WHERE [TenantId] = @TenantId AND [RoleCode] = N'Admin');

IF @RoleId IS NULL
BEGIN
    INSERT INTO [identity].[Identity_Role] (
        [TenantId], [FacilityId], [IsActive], [IsDeleted],
        [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy],
        [RoleCode], [RoleName], [Description], [IsSystemRole])
    VALUES (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, N'Admin', N'Administrator', NULL, 1);

    SET @RoleId = CAST(SCOPE_IDENTITY() AS BIGINT);
END;

INSERT INTO [identity].[Identity_RolePermission] (
    [RoleId], [PermissionId], [TenantId], [FacilityId],
    [IsActive], [IsDeleted], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy])
SELECT @RoleId, p.[Id], @TenantId, NULL, 1, 0, @Now, 0, @Now, 0
FROM [identity].[Identity_Permission] p
WHERE p.[TenantId] = @TenantId AND p.[IsDeleted] = 0
  AND NOT EXISTS (
        SELECT 1 FROM [identity].[Identity_RolePermission] rp
        WHERE rp.[RoleId] = @RoleId AND rp.[PermissionId] = p.[Id] AND rp.[TenantId] = @TenantId);

DECLARE @UserId BIGINT;

INSERT INTO [identity].[Identity_Users] (
    [Email], [PasswordHash], [TenantId], [FacilityId], [Role], [IsActive])
VALUES (@DemoEmail, @PasswordHash, @TenantId, 1, N'Admin', 1);

SET @UserId = CAST(SCOPE_IDENTITY() AS BIGINT);

INSERT INTO [identity].[Identity_UserProfile] (
    [UserId], [TenantId], [FacilityId], [IsActive], [IsDeleted],
    [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy],
    [DisplayName], [Phone], [PreferredLocaleReferenceValueId], [TimeZoneId], [AvatarUrl], [MfaEnabled])
VALUES (
    @UserId, @TenantId, 1, 1, 0, @Now, 0, @Now, 0,
    N'Demo Admin', NULL, NULL, NULL, NULL, 0);

INSERT INTO [identity].[Identity_UserFacilityGrant] (
    [TenantId], [FacilityId], [IsActive], [IsDeleted],
    [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy],
    [UserId], [GrantFacilityId])
VALUES (@TenantId, NULL, 1, 0, @Now, 0, @Now, 0, @UserId, 1);

INSERT INTO [identity].[Identity_UserRole] (
    [UserId], [RoleId], [TenantId], [FacilityId],
    [IsActive], [IsDeleted], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy])
VALUES (@UserId, @RoleId, @TenantId, NULL, 1, 0, @Now, 0, @Now, 0);

COMMIT TRANSACTION;

PRINT N'Seeded demo admin: ' + @DemoEmail + N' (password: ChangeMe!123, tenant ' + CAST(@TenantId AS NVARCHAR(20)) + N').';
GO
