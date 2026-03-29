using System.Reflection;
using CommunicationService.Application.Abstractions;
using CommunicationService.Application.Mapping;
using CommunicationService.Application.Services;
using CommunicationService.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCommunicationApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CommunicationMappingProfile).Assembly);
        services.AddScoped<ITemplateRenderer, TemplateRenderer>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
