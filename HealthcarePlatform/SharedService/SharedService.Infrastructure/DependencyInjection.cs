using Healthcare.Common.Events;
using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedService.Application.Services.Enterprise;
using SharedService.Infrastructure.Persistence;
using SharedService.Infrastructure.Services.Enterprise;

namespace SharedService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddSingleton<IEventPublisher, NoOpEventPublisher>();

        services.AddDbContext<SharedDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SharedDatabase")));

        services.AddScoped<IEnterpriseService, EnterpriseService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IBusinessUnitService, BusinessUnitService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IDepartmentService, DepartmentService>();

        return services;
    }
}
