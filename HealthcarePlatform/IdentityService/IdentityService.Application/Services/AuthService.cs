using System.Security.Claims;
using FluentValidation;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs;
using IdentityService.Application.Options;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.Rbac;
using IdentityService.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityService.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IValidator<LoginRequestDto> _validator;
    private readonly IRbacQueryService _rbac;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IAccountLockoutRepository _lockout;
    private readonly ILoginAuditRepository _loginAudit;
    private readonly IPasswordResetRepository _passwordReset;
    private readonly IUserProfileReadRepository _profileRead;
    private readonly IOptions<SecurityOptions> _security;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public AuthService(
        IUserRepository users,
        IJwtTokenGenerator jwt,
        IValidator<LoginRequestDto> validator,
        IRbacQueryService rbac,
        IRefreshTokenRepository refreshTokens,
        IAccountLockoutRepository lockout,
        ILoginAuditRepository loginAudit,
        IPasswordResetRepository passwordReset,
        IUserProfileReadRepository profileRead,
        IOptions<SecurityOptions> security,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment hostEnvironment)
    {
        _users = users;
        _jwt = jwt;
        _validator = validator;
        _rbac = rbac;
        _refreshTokens = refreshTokens;
        _lockout = lockout;
        _loginAudit = loginAudit;
        _passwordReset = passwordReset;
        _profileRead = profileRead;
        _security = security;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<BaseResponse<TokenResponseDto>> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BaseResponse<TokenResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));
        }

        var opt = _security.Value;
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var ip = GetClientIp();
        var user = await _users.GetByEmailAsync(normalizedEmail, request.TenantId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            await AuditLoginAsync(request.TenantId, null, normalizedEmail, false, ip, cancellationToken);
            await _users.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Failed login for unknown or inactive user {Email} tenant {TenantId}", normalizedEmail, request.TenantId);
            return BaseResponse<TokenResponseDto>.Fail("Invalid credentials.");
        }

        var lockState = await _lockout.GetAsync(user.Id, cancellationToken);
        if (lockState?.LockoutEndOn is { } end && end > DateTime.UtcNow)
        {
            await AuditLoginAsync(request.TenantId, user.Id, normalizedEmail, false, ip, cancellationToken);
            await _users.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Login rejected: account locked for user {UserId}", user.Id);
            return BaseResponse<TokenResponseDto>.Fail("Account is temporarily locked. Try again later.");
        }

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            await _lockout.RecordFailedAttemptAsync(user.Id, request.TenantId, opt.MaxFailedLoginAttempts, opt.LockoutMinutes, cancellationToken);
            await AuditLoginAsync(request.TenantId, user.Id, normalizedEmail, false, ip, cancellationToken);
            await _users.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Invalid password for user {UserId}", user.Id);
            return BaseResponse<TokenResponseDto>.Fail("Invalid credentials.");
        }

        if (await _profileRead.IsMfaEnabledAsync(user.Id, cancellationToken))
        {
            var bypass = opt.MfaDevBypassCode;
            if (string.IsNullOrEmpty(request.MfaCode) ||
                string.IsNullOrEmpty(bypass) ||
                !string.Equals(request.MfaCode, bypass, StringComparison.Ordinal))
            {
                await _lockout.RecordFailedAttemptAsync(user.Id, request.TenantId, opt.MaxFailedLoginAttempts, opt.LockoutMinutes, cancellationToken);
                await AuditLoginAsync(request.TenantId, user.Id, normalizedEmail, false, ip, cancellationToken);
                await _users.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("MFA validation failed for user {UserId}", user.Id);
                return BaseResponse<TokenResponseDto>.Fail("Multi-factor authentication is required. Provide a valid MFA code.");
            }
        }

        await _lockout.ClearFailuresAsync(user.Id, cancellationToken);

        var rbac = await _rbac.GetForUserAsync(user.Id, user.TenantId, cancellationToken);
        MergeLegacyRoleAndPermissions(user, rbac);

        var access = _jwt.CreateAccessToken(
            user,
            rbac.RoleCodes,
            rbac.PermissionCodes,
            rbac.FacilityGrantIds,
            opt.AccessTokenMinutes);

        var refreshRaw = await CreateAndPersistRefreshTokenAsync(user, ip, cancellationToken);

        await AuditLoginAsync(request.TenantId, user.Id, normalizedEmail, true, ip, cancellationToken);

        await _users.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} authenticated for tenant {TenantId}", user.Id, user.TenantId);

        return BaseResponse<TokenResponseDto>.Ok(new TokenResponseDto
        {
            AccessToken = access,
            ExpiresInSeconds = opt.AccessTokenMinutes * 60,
            TokenType = "Bearer",
            RefreshToken = refreshRaw,
            RefreshExpiresInSeconds = opt.RefreshTokenDays * 86400,
        });
    }

    public async Task<BaseResponse<TokenResponseDto>> RefreshTokenAsync(
        RefreshTokenRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BaseResponse<TokenResponseDto>.Fail("Refresh token is required.");

        var opt = _security.Value;
        var hash = SecureTokenUtility.Sha256Hex(request.RefreshToken.Trim());
        var existing = await _refreshTokens.FindActiveByHashAsync(hash, cancellationToken);
        if (existing is null)
        {
            _logger.LogWarning("Refresh rejected: token not found or expired");
            return BaseResponse<TokenResponseDto>.Fail("Invalid refresh token.");
        }

        var user = await _users.GetByIdAsync(existing.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            return BaseResponse<TokenResponseDto>.Fail("User is not active.");

        var now = DateTime.UtcNow;
        await _refreshTokens.RevokeAsync(existing.Id, now, null, cancellationToken);

        var rbac = await _rbac.GetForUserAsync(user.Id, user.TenantId, cancellationToken);
        MergeLegacyRoleAndPermissions(user, rbac);

        var access = _jwt.CreateAccessToken(
            user,
            rbac.RoleCodes,
            rbac.PermissionCodes,
            rbac.FacilityGrantIds,
            opt.AccessTokenMinutes);

        var ip = GetClientIp();
        var refreshRaw = await CreateAndPersistRefreshTokenAsync(user, ip, cancellationToken, existing.TokenFamilyId);

        await _refreshTokens.SaveChangesAsync(cancellationToken);

        return BaseResponse<TokenResponseDto>.Ok(new TokenResponseDto
        {
            AccessToken = access,
            ExpiresInSeconds = opt.AccessTokenMinutes * 60,
            TokenType = "Bearer",
            RefreshToken = refreshRaw,
            RefreshExpiresInSeconds = opt.RefreshTokenDays * 86400,
        });
    }

    public async Task<BaseResponse<object>> LogoutAsync(
        RefreshTokenRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BaseResponse<object>.Ok(new { message = "Logged out." });

        var hash = SecureTokenUtility.Sha256Hex(request.RefreshToken.Trim());
        var existing = await _refreshTokens.FindActiveByHashAsync(hash, cancellationToken);
        if (existing is not null)
            await _refreshTokens.RevokeAsync(existing.Id, DateTime.UtcNow, null, cancellationToken);

        await _refreshTokens.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Refresh token revoked (logout).");
        return BaseResponse<object>.Ok(new { message = "Logged out." });
    }

    public async Task<BaseResponse<object>> ForgotPasswordAsync(
        ForgotPasswordRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, request.TenantId, cancellationToken);
        if (user is null)
        {
            _logger.LogInformation("Password reset requested for unknown email (swallowed).");
            return BaseResponse<object>.Ok(new { message = "If the account exists, reset instructions will be sent." });
        }

        var raw = SecureTokenUtility.CreateUrlSafeToken();
        var hash = SecureTokenUtility.Sha256Hex(raw);
        var entity = new IdentityPasswordResetToken
        {
            TenantId = user.TenantId,
            UserId = user.Id,
            TokenHash = hash,
            ExpiresOn = DateTime.UtcNow.AddHours(1),
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
        };
        await _passwordReset.AddAsync(entity, cancellationToken);
        await _passwordReset.SaveChangesAsync(cancellationToken);

        if (_hostEnvironment.IsDevelopment())
        {
            _logger.LogWarning("DEV ONLY: password reset token for {Email}: {Token}", email, raw);
        }

        return BaseResponse<object>.Ok(new { message = "If the account exists, reset instructions will be sent." });
    }

    public async Task<BaseResponse<object>> ResetPasswordAsync(
        ResetPasswordRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, request.TenantId, cancellationToken);
        if (user is null)
            return BaseResponse<object>.Fail("Invalid reset request.");

        var hash = SecureTokenUtility.Sha256Hex(request.Token.Trim());
        var row = await _passwordReset.FindActiveByUserAndHashAsync(user.Id, hash, cancellationToken);
        if (row is null)
            return BaseResponse<object>.Fail("Invalid or expired reset token.");

        var trackedUser = await _users.GetByIdForUpdateAsync(user.Id, cancellationToken);
        if (trackedUser is null)
            return BaseResponse<object>.Fail("Invalid reset request.");

        trackedUser.PasswordHash = _passwordHasher.HashPassword(trackedUser, request.NewPassword);
        _users.Update(trackedUser);
        row.ConsumedOn = DateTime.UtcNow;
        row.ModifiedOn = DateTime.UtcNow;

        await _users.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Password reset completed for user {UserId}", user.Id);
        return BaseResponse<object>.Ok(new { message = "Password updated." });
    }

    public BaseResponse<UserSummaryDto> GetCurrentUser(ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub");
        if (string.IsNullOrEmpty(sub) || !long.TryParse(sub, out var userId))
            return BaseResponse<UserSummaryDto>.Fail("Invalid token subject.");

        var tenant = principal.FindFirstValue(TriVitaClaimTypes.TenantId);
        var facility = principal.FindFirstValue(TriVitaClaimTypes.FacilityId);
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var permissions = principal.FindAll(TriVitaClaimTypes.Permission).Select(c => c.Value).ToList();

        return BaseResponse<UserSummaryDto>.Ok(new UserSummaryDto
        {
            UserId = userId,
            Email = principal.FindFirstValue(ClaimTypes.Email)
                    ?? principal.FindFirstValue("email")
                    ?? string.Empty,
            TenantId = long.TryParse(tenant, out var t) ? t : 0,
            FacilityId = long.TryParse(facility, out var f) ? f : null,
            Role = roles.FirstOrDefault() ?? string.Empty,
            Roles = roles,
            Permissions = permissions,
        });
    }

    private string? GetClientIp() =>
        _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    private async Task AuditLoginAsync(
        long tenantId,
        long? userId,
        string email,
        bool success,
        string? ip,
        CancellationToken cancellationToken)
    {
        var attempt = new IdentityLoginAttempt
        {
            TenantId = tenantId,
            UserId = userId,
            EmailAttempted = email,
            Success = success,
            IpAddress = ip,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
        };
        await _loginAudit.AppendAsync(attempt, cancellationToken);
    }

    private async Task<string> CreateAndPersistRefreshTokenAsync(
        AppUser user,
        string? ip,
        CancellationToken cancellationToken,
        Guid? existingFamily = null)
    {
        var opt = _security.Value;
        var family = existingFamily ?? Guid.NewGuid();
        var raw = SecureTokenUtility.CreateUrlSafeToken();
        var hash = SecureTokenUtility.Sha256Hex(raw);
        var entity = new IdentityRefreshToken
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            FacilityId = user.FacilityId,
            TokenFamilyId = family,
            TokenHash = hash,
            ExpiresOn = DateTime.UtcNow.AddDays(opt.RefreshTokenDays),
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
            ClientIp = ip,
        };
        await _refreshTokens.AddAsync(entity, cancellationToken);
        return raw;
    }

    private static void MergeLegacyRoleAndPermissions(AppUser user, RbacPrincipalData rbac)
    {
        if (!rbac.RoleCodes.Any() && !string.IsNullOrWhiteSpace(user.Role))
            rbac.RoleCodes.Add(user.Role);

        if (!rbac.PermissionCodes.Any() &&
            user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            rbac.PermissionCodes.Add(TriVitaPermissions.Wildcard);
    }
}
