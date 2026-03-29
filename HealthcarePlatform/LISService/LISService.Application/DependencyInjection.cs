using System.Reflection;
using FluentValidation;
using LISService.Application.Abstractions;
using LISService.Application.Mapping;
using LISService.Application.Services;
using LISService.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LISService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddLisApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(LISMappingProfile), typeof(LisGeneratedMappingProfile));
        services.AddTransient(typeof(IValidator<>), typeof(NoOpValidator<>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IInfoService, InfoService>();
        services.AddScoped<ILisNotificationHelper, LisNotificationHelper>();

        RegisterEntityCrudServices(services);

        return services;
    }

    private static void RegisterEntityCrudServices(IServiceCollection services)
    {
        const string ns = "LISService.Application.Services.Entities";
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
