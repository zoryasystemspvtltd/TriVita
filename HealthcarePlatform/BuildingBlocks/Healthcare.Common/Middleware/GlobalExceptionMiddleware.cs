using System.Net;
using System.Text.Json;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Healthcare.Common.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            long? tenantId = null;
            long? facilityId = null;
            try
            {
                var tenant = context.RequestServices.GetService<ITenantContext>();
                if (tenant is not null)
                {
                    tenantId = tenant.TenantId;
                    facilityId = tenant.FacilityId;
                }
            }
            catch
            {
                // ignore resolution failures
            }

            _logger.LogError(
                ex,
                "Unhandled exception Path={Path} TraceId={TraceId} TenantId={TenantId} FacilityId={FacilityId}",
                context.Request.Path.Value,
                context.TraceIdentifier,
                tenantId,
                facilityId);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var body = BaseResponse<object>.Fail("An unexpected error occurred.");
            await context.Response.WriteAsync(JsonSerializer.Serialize(body,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) =>
        app.UseMiddleware<GlobalExceptionMiddleware>();
}
