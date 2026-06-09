using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Tests.Application.Queries;

public class GetSpaceObjectByIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_NotFound_ThrowsSpaceObjectNotFoundException()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        var repoMock = new Mock<ISpaceObjectRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SpaceObject?)null);

        var handler = new GetSpaceObjectByIdQueryHandler(repoMock.Object);
        var act = async () => await handler.HandleAsync(new GetSpaceObjectByIdQuery(Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<SpaceObjectNotFoundException>();
    }
}
