using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Tests.Domain.Aggregates;

public class UserTests
{
    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var user = User.Create("jo@test.com", "hash123", "Jo Furtado", UserRole.Admin);
        user.Email.Should().Be("jo@test.com");
        user.FullName.Should().Be("Jo Furtado");
        user.Role.Should().Be(UserRole.Admin);
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidEmail_ThrowsException()
    {
        var act = () => User.Create("not-an-email", "hash", "Name", UserRole.Analyst);
        act.Should().Throw<DuplicateEmailException>();
    }

    [Fact]
    public void UpdateProfile_ChangesFullName()
    {
        var user = User.Create("jo@test.com", "hash", "Old Name", UserRole.Operator);
        user.UpdateProfile("New Name");
        user.FullName.Should().Be("New Name");
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var user = User.Create("jo@test.com", "hash", "Jo", UserRole.Admin);
        user.Deactivate();
        user.IsActive.Should().BeFalse();
    }
}
