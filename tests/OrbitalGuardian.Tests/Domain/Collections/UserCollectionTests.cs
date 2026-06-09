using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Tests.Domain.Collections;

public class UserCollectionTests
{
    private static User MakeUser(string email, UserRole role) =>
        User.Create(email, "hash", "Full Name", role);

    [Fact]
    public void FilterByRole_Admin_ReturnsOnlyAdmins()
    {
        var col = new UserCollection(new[]
        {
            MakeUser("admin@x.com", UserRole.Admin),
            MakeUser("op@x.com", UserRole.Operator)
        });
        col.FilterByRole(UserRole.Admin).Should().HaveCount(1);
    }

    [Fact]
    public void FindByEmail_CaseInsensitive_ReturnsUser()
    {
        var col = new UserCollection(new[] { MakeUser("Admin@X.com", UserRole.Admin) });
        col.FindByEmail("admin@x.com").Should().NotBeNull();
    }

    [Fact]
    public void FindByEmail_NonExisting_ReturnsNull()
    {
        var col = new UserCollection(Array.Empty<User>());
        col.FindByEmail("nobody@x.com").Should().BeNull();
    }
}
