using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Healthcare.Common.Authorization;
using Healthcare.Common.Hosting;
using Healthcare.Common.Middleware;
using Healthcare.Swagger;
using IdentityService.Application;
using IdentityService.Application.DTOs;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
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

builder.Services.AddAuthorization();
builder.Services.AddTriVitaPermissionAuthorization();

builder.Services.AddTriVitaSwagger(
    "IdentityService API",
    "v1",
    typeof(Program).Assembly,
    typeof(LoginRequestDto).Assembly);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<IdentityDbContext>();

builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

builder.AddTriVitaPortalCorsIfConfigured();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    IdentityDataSeeder.Seed(app.Services);
}

app.UseGlobalExceptionHandler();

app.UseTriVitaSwaggerUi("v1", "IdentityService v1");

app.UseTriVitaCorsAndHttpsRedirection();

app.UseAuthentication();
app.UseTriVitaSecurityContextAlignment();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
