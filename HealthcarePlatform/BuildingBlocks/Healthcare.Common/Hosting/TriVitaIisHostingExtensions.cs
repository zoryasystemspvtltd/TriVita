using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Healthcare.Common.Hosting;

/// <summary>
/// Optional IIS / reverse-proxy deployment: path base, CORS for the SPA origin, and HTTPS redirection toggle (configuration only).
/// </summary>
public static class TriVitaIisHostingExtensions
{
    public const string CorsPolicyName = "TriVitaPortal";

    /// <summary>Registers CORS when <c>Cors:AllowedOrigins</c> is non-empty (e.g. Production IIS + separate SPA port).</summary>
    public static WebApplicationBuilder AddTriVitaPortalCorsIfConfigured(this WebApplicationBuilder builder)
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (origins is not { Length: > 0 }) return builder;

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return builder;
    }

    /// <summary>Apply first in the pipeline when <c>IIS:PathBase</c> is set (e.g. <c>/hms</c>).</summary>
    public static WebApplication UseTriVitaIisPathBase(this WebApplication app)
    {
        var raw = app.Configuration["IIS:PathBase"];
        if (string.IsNullOrWhiteSpace(raw)) return app;

        var path = raw.Trim();
        if (!path.StartsWith('/')) path = "/" + path;

        app.UsePathBase(new PathString(path));
        return app;
    }

    /// <summary>CORS (if configured) then optional HTTPS redirection. Call before <c>UseAuthentication</c>.</summary>
    public static WebApplication UseTriVitaCorsAndHttpsRedirection(this WebApplication app)
    {
        var origins = app.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (origins is { Length: > 0 }) app.UseCors(CorsPolicyName);

        var disableHttps = app.Configuration.GetValue<bool>("DisableHttpsRedirection");
        if (!disableHttps) app.UseHttpsRedirection();

        return app;
    }
}
