using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Application.Commands;

public class DeleteSpaceObjectCommandHandlerTests
{
    private readonly Mock<ISpaceObjectRepository> _repoMock = new();

    private DeleteSpaceObjectCommandHandler CreateHandler()
    {
        OrbitalLogger.Initialize(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        return new(_repoMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ExistingSpaceObject_DeactivatesAndCallsUpdateAsync()
    {
        var satellite = Satellite.Create("ISS", "25544", DateTime.UtcNow.AddYears(-10),
            OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300),
            "NASA", "Research", 420000);
        _repoMock.Setup(r => r.GetByIdAsync(satellite.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(satellite);

        var result = await CreateHandler().HandleAsync(new DeleteSpaceObjectCommand(satellite.Id), CancellationToken.None);

        result.Should().BeTrue();
        satellite.IsActive.Should().BeFalse();
        _repoMock.Verify(r => r.UpdateAsync(satellite, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NonExistentSpaceObject_ThrowsSpaceObjectNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SpaceObject?)null);

        var act = async () => await CreateHandler().HandleAsync(new DeleteSpaceObjectCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<SpaceObjectNotFoundException>();
    }
}
