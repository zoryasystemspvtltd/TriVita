using System.Reflection;
using FluentValidation;
using LMSService.Application.Abstractions;
using LMSService.Application.Mapping;
using LMSService.Application.Services;
using LMSService.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LMSService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddLmsApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(LMSMappingProfile), typeof(LmsGeneratedMappingProfile), typeof(Lms07ExtendedMappingProfile));
        services.AddTransient(typeof(IValidator<>), typeof(NoOpValidator<>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IInfoService, InfoService>();
        services.AddScoped<ILmsNotificationHelper, LmsNotificationHelper>();

        RegisterEntityCrudServices(services);

        return services;
    }

    private static void RegisterEntityCrudServices(IServiceCollection services)
    {
        const string ns = "LMSService.Application.Services.Entities";
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
