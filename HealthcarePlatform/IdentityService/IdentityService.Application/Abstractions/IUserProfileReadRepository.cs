namespace IdentityService.Application.Abstractions;

public interface IUserProfileReadRepository
{
    Task<bool> IsMfaEnabledAsync(long userId, CancellationToken cancellationToken = default);
}
