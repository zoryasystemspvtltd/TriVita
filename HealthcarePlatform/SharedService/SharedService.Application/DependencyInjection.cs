using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharedService.Application.Mapping;
using SharedService.Application.Services;

namespace SharedService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(SharedMappingProfile));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IInfoService, InfoService>();

        return services;
    }
}
