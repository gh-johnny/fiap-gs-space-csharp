using OrbitalGuardian.Domain.Aggregates.Users;

namespace OrbitalGuardian.Application.Interfaces;

/// <summary>Repository interface for User aggregate persistence.</summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<UserCollection> GetAllAsync(CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task UpdateAsync(User user, CancellationToken ct);
}
