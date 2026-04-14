using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Healthcare.Swagger;

/// <summary>
/// Shared OpenAPI/Swagger registration for TriVita microservices (JWT Bearer, XML docs, annotations).
/// </summary>
public static class TriVitaSwaggerExtensions
{
    /// <summary>
    /// Registers Swagger/OpenAPI with document name <paramref name="documentName"/> (e.g. v1),
    /// enables Swashbuckle annotations, includes XML documentation from the given assemblies when present in output,
    /// and configures Bearer JWT security for Swagger UI "Authorize".
    /// </summary>
    public static IServiceCollection AddTriVitaSwagger(
        this IServiceCollection services,
        string apiTitle,
        string documentName = "v1",
        params Assembly[] xmlDocumentationAssemblies)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(documentName, new OpenApiInfo
            {
                Title = apiTitle,
                Version = documentName,
                Description =
                    "TriVita Healthcare Platform — REST API. Use **Authorize** and paste a Bearer token from IdentityService."
            });

            options.EnableAnnotations();

            foreach (var assembly in xmlDocumentationAssemblies)
            {
                var xml = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
                if (File.Exists(xml))
                    options.IncludeXmlComments(xml, includeControllerXmlComments: true);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste **Bearer &lt;token&gt;** or use Authorize and enter only the raw JWT."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });

            options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
        });

        return services;
    }

    /// <summary>Maps Swagger UI for a single OpenAPI document (e.g. v1).</summary>
    public static IApplicationBuilder UseTriVitaSwaggerUi(
        this IApplicationBuilder app,
        string documentName = "v1",
        string? title = null)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // Relative URL so Swagger UI works when the site is hosted as an IIS application (/hms/swagger, /identity/swagger, …).
            options.SwaggerEndpoint($"{documentName}/swagger.json", title ?? $"API {documentName}");
            options.DocumentTitle = $"{title ?? "TriVita"} — Swagger";
            options.DisplayRequestDuration();
        });

        return app;
    }
}
