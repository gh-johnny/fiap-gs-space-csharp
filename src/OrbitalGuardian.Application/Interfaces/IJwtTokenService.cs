using OrbitalGuardian.Domain.Aggregates.Users;

namespace OrbitalGuardian.Application.Interfaces;

/// <summary>Generates a JWT bearer token for the given user.</summary>
public interface IJwtTokenService
{
    string GenerateToken(User user);
}
