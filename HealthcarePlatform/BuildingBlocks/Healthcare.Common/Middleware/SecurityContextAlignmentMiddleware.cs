using System.Security.Claims;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Healthcare.Common.Middleware;

/// <summary>
/// When authenticated, ensures <c>X-Tenant-Id</c> and <c>X-Facility-Id</c> align with JWT claims
/// (tenant must match; facility must match primary <c>facility_id</c> or an <c>allowed_facility</c> claim,
/// unless the user is Admin or has the <c>*</c> permission).
/// </summary>
public sealed class SecurityContextAlignmentMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityContextAlignmentMiddleware> _logger;

    public SecurityContextAlignmentMiddleware(
        RequestDelegate next,
        ILogger<SecurityContextAlignmentMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (IsExcluded(path))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        if (!ValidateTenant(context))
        {
            _logger.LogWarning("Rejected request: X-Tenant-Id does not match token for {Path}", path);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Tenant header does not match the authenticated token."
            });
            return;
        }

        if (!ValidateFacility(context))
        {
            _logger.LogWarning("Rejected request: X-Facility-Id not allowed for user on {Path}", path);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Facility header is not in scope for this user."
            });
            return;
        }

        await _next(context);
    }

    private static bool IsExcluded(string path)
    {
        if (string.IsNullOrEmpty(path))
            return true;
        if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
            return true;
        if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            return true;
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    private static bool ValidateTenant(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValues))
            return true;

        var headerText = headerValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(headerText) || !long.TryParse(headerText, out var headerTenant))
            return true;

        var claimTenant = FindClaim(context.User, TriVitaClaimTypes.TenantId);
        if (!long.TryParse(claimTenant, out var claimT) || claimT == 0)
            return true;

        return headerTenant == claimT;
    }

    private static bool ValidateFacility(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Facility-Id", out var headerValues))
            return true;

        var headerText = headerValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(headerText) || !long.TryParse(headerText, out var headerFacility))
            return true;

        if (context.User.IsInRole("Admin"))
            return true;

        if (context.User.HasClaim(TriVitaClaimTypes.Permission, TriVitaPermissions.Wildcard))
            return true;

        var primary = FindClaim(context.User, TriVitaClaimTypes.FacilityId);
        if (long.TryParse(primary, out var p) && p == headerFacility)
            return true;

        foreach (var c in context.User.FindAll(TriVitaClaimTypes.AllowedFacility))
        {
            if (long.TryParse(c.Value, out var af) && af == headerFacility)
                return true;
        }

        return false;
    }

    private static string? FindClaim(ClaimsPrincipal user, string type) =>
        user.Claims.FirstOrDefault(c => c.Type == type)?.Value;
}

public static class SecurityContextAlignmentMiddlewareExtensions
{
    public static IApplicationBuilder UseTriVitaSecurityContextAlignment(this IApplicationBuilder app) =>
        app.UseMiddleware<SecurityContextAlignmentMiddleware>();
}
