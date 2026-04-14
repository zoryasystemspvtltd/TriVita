using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Healthcare.Common.Hosting;

/// <summary>
/// Optional IIS / reverse-proxy deployment: CORS for the SPA origin and HTTPS redirection toggle (configuration only).
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
