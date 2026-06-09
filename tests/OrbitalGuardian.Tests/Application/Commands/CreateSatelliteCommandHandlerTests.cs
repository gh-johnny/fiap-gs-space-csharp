using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Tests.Application.Commands;

public class CreateSatelliteCommandHandlerTests
{
    private readonly Mock<ISpaceObjectRepository> _repoMock = new();
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock = new();

    [Fact]
    public async Task HandleAsync_ValidCommand_CallsAddAsyncOnce()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        var handler = new CreateSatelliteCommandHandler(_repoMock.Object, _dispatcherMock.Object);
        var command = new CreateSatelliteCommand(
            "TestSat", "99001", DateTime.UtcNow.AddYears(-1),
            "NASA", "Research", 500,
            51.6, 0.0001, 15.5, 100, 200, 300);

        var response = await handler.HandleAsync(command, CancellationToken.None);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<OrbitalGuardian.Domain.Aggregates.SpaceObjects.SpaceObject>(), It.IsAny<CancellationToken>()), Times.Once);
        _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
        response.Name.Should().Be("TestSat");
    }
}
