using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Tests.Application.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IJwtTokenService> _jwtMock = new();

    private LoginCommandHandler CreateHandler()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        return new LoginCommandHandler(_repoMock.Object, _hasherMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task HandleAsync_EmailNotFound_ThrowsInvalidCredentialsException()
    {
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var act = async () => await handler.HandleAsync(new LoginCommand("x@x.com", "pw"), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task HandleAsync_WrongPassword_ThrowsInvalidCredentialsException()
    {
        var user = User.Create("jo@test.com", "correct_hash", "Jo", UserRole.Admin);
        _repoMock.Setup(r => r.GetByEmailAsync("jo@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify("wrong", "correct_hash")).Returns(false);

        var handler = CreateHandler();
        var act = async () => await handler.HandleAsync(new LoginCommand("jo@test.com", "wrong"), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task HandleAsync_CorrectCredentials_ReturnsTokenResponse()
    {
        var user = User.Create("jo@test.com", "hash", "Jo", UserRole.Admin);
        _repoMock.Setup(r => r.GetByEmailAsync("jo@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify("pw", "hash")).Returns(true);
        _jwtMock.Setup(j => j.GenerateToken(user)).Returns("jwt_token_abc");

        var handler = CreateHandler();
        var response = await handler.HandleAsync(new LoginCommand("jo@test.com", "pw"), CancellationToken.None);
        response.Token.Should().Be("jwt_token_abc");
    }
}
