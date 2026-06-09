using Microsoft.EntityFrameworkCore;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Infrastructure.Persistence;
using Polly;

namespace OrbitalGuardian.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OrbitalGuardianDbContext _context;
    private readonly IAsyncPolicy _policy;

    public UserRepository(OrbitalGuardianDbContext context)
    {
        _context = context;
        _policy = Policy.WrapAsync(
            RepositoryPolicy.CreateRetryPolicy(),
            RepositoryPolicy.CreateTimeoutPolicy());
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _policy.ExecuteAsync(ct => _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct), ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        await _policy.ExecuteAsync(ct =>
            _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct), ct);

    public async Task<UserCollection> GetAllAsync(CancellationToken ct)
    {
        var results = await _policy.ExecuteAsync(ct => _context.Users.ToListAsync(ct), ct);
        return new UserCollection(results);
    }

    public async Task AddAsync(User user, CancellationToken ct) =>
        await _policy.ExecuteAsync(async ct =>
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }, ct);

    public async Task UpdateAsync(User user, CancellationToken ct) =>
        await _policy.ExecuteAsync(async ct =>
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
        }, ct);
}
