using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Healthcare.Common.Authentication;
using Healthcare.Common.Authorization;
using Healthcare.Common.Hosting;
using Healthcare.Common.Middleware;
using Healthcare.Swagger;
using HMSService.Application;
using HMSService.Application.DTOs.Appointments;
using HMSService.Infrastructure;
using HMSService.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(cs))
{
    var csb = new SqlConnectionStringBuilder(cs);
    Console.WriteLine($"DB: Server={csb.DataSource}; Database={csb.InitialCatalog}");
}
else
{
    Console.WriteLine("DB: DefaultConnection is missing.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

var useTestAuth = builder.Configuration.GetValue<bool>("IntegrationTest:UseTestAuth");
if (useTestAuth)
{
    builder.Services.AddAuthentication("Test")
        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", _ => { });
}
else
{
    var jwtKey = builder.Configuration["Jwt:Key"]!;
    var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
    var jwtAudience = builder.Configuration["Jwt:Audience"]!;

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        });
}

builder.Services.AddAuthorization();
builder.Services.AddTriVitaPermissionAuthorization();

builder.Services.AddTriVitaSwagger(
    "HMSService API",
    "v1",
    typeof(Program).Assembly,
    typeof(CreateAppointmentDto).Assembly);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<HmsDbContext>();

builder.Services.AddHmsApplication();
builder.Services.AddHmsInfrastructure(builder.Configuration);

builder.AddTriVitaPortalCorsIfConfigured();

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseTriVitaSwaggerUi("v1", "HMSService v1");

app.UseTriVitaCorsAndHttpsRedirection();

app.UseAuthentication();
app.UseTriVitaSecurityContextAlignment();
app.UseAuthorization();

app.UseRequireTenantContext();

app.UseRequestLogging();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
