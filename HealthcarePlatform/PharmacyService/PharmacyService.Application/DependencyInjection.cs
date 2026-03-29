using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PharmacyService.Application.Abstractions;
using PharmacyService.Application.Mapping;
using PharmacyService.Application.Services;
using PharmacyService.Application.Validation;

namespace PharmacyService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPharmacyApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(PharmacyMappingProfile), typeof(PhrGeneratedMappingProfile));
        services.AddTransient(typeof(IValidator<>), typeof(NoOpValidator<>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IInfoService, InfoService>();
        services.AddScoped<IPharmacyNotificationHelper, PharmacyNotificationHelper>();
        services.AddScoped<ILmsInventoryIntegrationService, LmsInventoryIntegrationService>();

        RegisterEntityCrudServices(services);

        return services;
    }

    private static void RegisterEntityCrudServices(IServiceCollection services)
    {
        const string ns = "PharmacyService.Application.Services.Entities";
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes()
            .Where(t =>
                t.Namespace == ns &&
                t is { IsClass: true, IsAbstract: false } &&
                t.Name.EndsWith("Service", StringComparison.Ordinal)))
        {
            var iface = type.GetInterfaces().FirstOrDefault(i => i.Name == "I" + type.Name);
            if (iface is not null)
                services.AddScoped(iface, type);
        }
    }
}
