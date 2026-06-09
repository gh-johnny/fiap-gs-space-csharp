using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Tests.Application.Commands;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock = new();

    private DeleteUserCommandHandler CreateHandler() => new(_repoMock.Object);

    [Fact]
    public async Task HandleAsync_ExistingUser_DeactivatesAndCallsUpdateAsync()
    {
        var user = User.Create("jo@test.com", "hash", "Jo", UserRole.Analyst);
        _repoMock.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await CreateHandler().HandleAsync(new DeleteUserCommand(user.Id), CancellationToken.None);

        result.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        _repoMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NonExistentUser_ThrowsUserNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var act = async () => await CreateHandler().HandleAsync(new DeleteUserCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }
}
