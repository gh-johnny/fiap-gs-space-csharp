using Microsoft.EntityFrameworkCore;
using OrbitalGuardian.Infrastructure.Auth;
using OrbitalGuardian.Infrastructure.Persistence;

namespace OrbitalGuardian.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<OrbitalGuardianDbContext>();
        await db.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}
