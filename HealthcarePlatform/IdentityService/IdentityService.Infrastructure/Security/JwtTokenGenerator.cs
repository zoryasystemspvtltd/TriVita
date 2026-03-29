using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Application.Abstractions;
using IdentityService.Domain.Entities;
using Healthcare.Common.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateAccessToken(
        AppUser user,
        IReadOnlyList<string> roleCodes,
        IReadOnlyList<string> permissionCodes,
        IReadOnlyList<long> extraFacilityIds,
        int lifetimeMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(TriVitaClaimTypes.TenantId, user.TenantId.ToString()),
        };

        foreach (var r in roleCodes.Distinct(StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim(ClaimTypes.Role, r));

        foreach (var p in permissionCodes.Distinct(StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim(TriVitaClaimTypes.Permission, p));

        if (user.FacilityId is not null)
            claims.Add(new Claim(TriVitaClaimTypes.FacilityId, user.FacilityId.Value.ToString()));

        foreach (var f in extraFacilityIds.Distinct())
        {
            if (user.FacilityId == f)
                continue;
            claims.Add(new Claim(TriVitaClaimTypes.AllowedFacility, f.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(lifetimeMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
