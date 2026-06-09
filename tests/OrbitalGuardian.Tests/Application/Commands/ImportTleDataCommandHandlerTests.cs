using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Tests.Application.Commands;

public class ImportTleDataCommandHandlerTests
{
    private static readonly string Line1 = "1 25544U 98067A   21275.52069444  .00001520  00000-0  35029-4 0  9993";
    private static readonly string Line2 = "2 25544  51.6442 337.6640 0001772  35.5820 324.5240 15.50377579303371";

    [Fact]
    public async Task HandleAsync_NewObjects_CallsAddAsyncForEach()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        var gatewayMock = new Mock<ITleDataGateway>();
        var repoMock = new Mock<ISpaceObjectRepository>();
        var dispatcherMock = new Mock<IDomainEventDispatcher>();

        gatewayMock.Setup(g => g.FetchTleDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SpaceObjectTleDto>
            {
                new() { Name = "SAT-A", NoradId = "11111", ObjectType = "PAYLOAD", Line1 = Line1, Line2 = Line2 },
                new() { Name = "SAT-B", NoradId = "22222", ObjectType = "PAYLOAD", Line1 = Line1, Line2 = Line2 }
            });

        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpaceObjectCollection(Array.Empty<SpaceObject>()));

        var handler = new ImportTleDataCommandHandler(gatewayMock.Object, repoMock.Object, dispatcherMock.Object);
        var results = await handler.HandleAsync(new ImportTleDataCommand(), CancellationToken.None);

        repoMock.Verify(r => r.AddAsync(It.IsAny<SpaceObject>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        results.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleAsync_ExistingObject_CallsUpdateAsync()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        var gatewayMock = new Mock<ITleDataGateway>();
        var repoMock = new Mock<ISpaceObjectRepository>();
        var dispatcherMock = new Mock<IDomainEventDispatcher>();

        var oe = OrbitalGuardian.Domain.ValueObjects.OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300);
        var existing = Satellite.Create("ISS", "25544", DateTime.UtcNow.AddYears(-10), oe, "NASA", "Research", 420_000);

        gatewayMock.Setup(g => g.FetchTleDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SpaceObjectTleDto>
            {
                new() { Name = "ISS", NoradId = "25544", ObjectType = "PAYLOAD", Line1 = Line1, Line2 = Line2 }
            });

        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpaceObjectCollection(new[] { existing }));

        var handler = new ImportTleDataCommandHandler(gatewayMock.Object, repoMock.Object, dispatcherMock.Object);
        await handler.HandleAsync(new ImportTleDataCommand(), CancellationToken.None);

        repoMock.Verify(r => r.UpdateAsync(It.IsAny<SpaceObject>(), It.IsAny<CancellationToken>()), Times.Once);
        repoMock.Verify(r => r.AddAsync(It.IsAny<SpaceObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
