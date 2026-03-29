using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Healthcare.Common.Authorization;

public static class TriVitaAuthorizationExtensions
{
    /// <summary>
    /// Registers permission-based authorization (dynamic <c>Permission:*</c> policies).
    /// Call after <see cref="AuthorizationServiceCollectionExtensions.AddAuthorization"/>.
    /// </summary>
    public static IServiceCollection AddTriVitaPermissionAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.RemoveAll<IAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        return services;
    }
}
