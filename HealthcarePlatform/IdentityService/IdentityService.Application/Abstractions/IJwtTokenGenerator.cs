using IdentityService.Domain.Entities;

namespace IdentityService.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string CreateAccessToken(
        AppUser user,
        IReadOnlyList<string> roleCodes,
        IReadOnlyList<string> permissionCodes,
        IReadOnlyList<long> extraFacilityIds,
        int lifetimeMinutes);
}
