using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Infrastructure.Persistence;

namespace OrbitalGuardian.Infrastructure.Auth;

public class DatabaseSeeder
{
    private readonly OrbitalGuardianDbContext _context;
    private readonly IPasswordHasher _hasher;

    public DatabaseSeeder(OrbitalGuardianDbContext context, IPasswordHasher hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public async Task SeedAsync()
    {
        var adminExists = _context.Users.Any(u => u.Role == UserRole.Admin);
        if (adminExists) return;

        var hash = _hasher.Hash("Admin@123");
        var admin = User.Create("admin@orbitalguardian.com", hash, "System Administrator", UserRole.Admin);

        await _context.Users.AddAsync(admin);
        await _context.SaveChangesAsync();

        OrbitalLogger.LogInfo("DatabaseSeeder", "Default admin user created.");
    }
}
