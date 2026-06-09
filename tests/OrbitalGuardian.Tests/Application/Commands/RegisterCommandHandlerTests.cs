using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Tests.Application.Commands;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();

    private RegisterCommandHandler CreateHandler()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        return new RegisterCommandHandler(_repoMock.Object, _hasherMock.Object);
    }

    [Fact]
    public async Task HandleAsync_DuplicateEmail_ThrowsDuplicateEmailException()
    {
        var existing = User.Create("jo@test.com", "hash", "Jo", UserRole.Admin);
        _repoMock.Setup(r => r.GetByEmailAsync("jo@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = CreateHandler();
        var act = async () => await handler.HandleAsync(
            new RegisterCommand("jo@test.com", "pass", "Jo", UserRole.Admin), CancellationToken.None);
        await act.Should().ThrowAsync<DuplicateEmailException>();
    }

    [Fact]
    public async Task HandleAsync_NewEmail_CallsAddAsync()
    {
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _hasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");

        var handler = CreateHandler();
        var response = await handler.HandleAsync(
            new RegisterCommand("new@test.com", "pass", "New User", UserRole.Analyst), CancellationToken.None);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        response.Email.Should().Be("new@test.com");
    }

    [Fact]
    public async Task HandleAsync_NewEmail_PasswordIsHashed()
    {
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _hasherMock.Setup(h => h.Hash("rawpassword")).Returns("hashed_pw");

        var handler = CreateHandler();
        await handler.HandleAsync(
            new RegisterCommand("x@test.com", "rawpassword", "X", UserRole.Operator), CancellationToken.None);

        _hasherMock.Verify(h => h.Hash("rawpassword"), Times.Once);
    }
}
