using Healthcare.Common.MultiTenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Healthcare.Common.Middleware;

/// <summary>
/// Ensures <see cref="ITenantContext.TenantId"/> is resolved for versioned API routes.
/// Health, Swagger, and root are excluded.
/// </summary>
public sealed class RequireTenantContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequireTenantContextMiddleware> _logger;

    public RequireTenantContextMiddleware(RequestDelegate next, ILogger<RequireTenantContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (IsExcluded(path))
        {
            await _next(context);
            return;
        }

        if (tenantContext.TenantId == 0)
        {
            _logger.LogWarning("Missing tenant context for {Path}", path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Tenant context is required. Send X-Tenant-Id header or include tenant_id claim."
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

        // Non-API routes (e.g. favicon)
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}

public static class RequireTenantContextMiddlewareExtensions
{
    public static IApplicationBuilder UseRequireTenantContext(this IApplicationBuilder app) =>
        app.UseMiddleware<RequireTenantContextMiddleware>();
}
