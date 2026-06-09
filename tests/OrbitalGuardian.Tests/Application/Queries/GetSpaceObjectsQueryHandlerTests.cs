using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Application.Queries;

public class GetSpaceObjectsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsMappedResponse()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        var repoMock = new Mock<ISpaceObjectRepository>();
        var oe = OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300);
        var sat = Satellite.Create("ISS", "25544", DateTime.UtcNow.AddYears(-10), oe, "NASA", "Research", 420_000);

        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpaceObjectCollection(new[] { sat }));

        var handler = new GetSpaceObjectsQueryHandler(repoMock.Object);
        var result = await handler.HandleAsync(new GetSpaceObjectsQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("ISS");
    }
}
