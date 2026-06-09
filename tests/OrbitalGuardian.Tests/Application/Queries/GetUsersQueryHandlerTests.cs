using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Tests.Application.Queries;

public class GetUsersQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsMappedUserResponses()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        var repoMock = new Mock<IUserRepository>();
        var users = new UserCollection(new[]
        {
            User.Create("admin@x.com", "hash", "Admin", UserRole.Admin),
            User.Create("op@x.com", "hash", "Operator", UserRole.Operator)
        });

        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var handler = new GetUsersQueryHandler(repoMock.Object);
        var result = await handler.HandleAsync(new GetUsersQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "admin@x.com");
    }
}
