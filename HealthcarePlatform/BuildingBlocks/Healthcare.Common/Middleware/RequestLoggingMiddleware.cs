using Healthcare.Common.MultiTenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Healthcare.Common.Middleware;

/// <summary>
/// Structured request/response logging for API routes after tenant context is available.
/// Place after <see cref="RequireTenantContextMiddlewareExtensions.UseRequireTenantContext"/>.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenant)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        _logger.LogInformation(
            "HTTP request {Method} {Path} TenantId={TenantId} FacilityId={FacilityId}",
            context.Request.Method,
            path,
            tenant.TenantId,
            tenant.FacilityId);

        await _next(context);

        _logger.LogInformation(
            "HTTP response {Method} {Path} Status={StatusCode} TenantId={TenantId}",
            context.Request.Method,
            path,
            context.Response.StatusCode,
            tenant.TenantId);
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestLoggingMiddleware>();
}
