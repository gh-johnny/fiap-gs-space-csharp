using FluentAssertions;
using Moq;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Application.Commands;

public class DeleteConjunctionEventCommandHandlerTests
{
    private readonly Mock<IConjunctionEventRepository> _repoMock = new();

    private DeleteConjunctionEventCommandHandler CreateHandler() => new(_repoMock.Object);

    [Fact]
    public async Task HandleAsync_ExistingEvent_CallsDeleteAsync()
    {
        var conjunction = ConjunctionEvent.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddHours(2),
            MissDistance.Create(0.5, 3600), CollisionProbability.Create(0.01));
        _repoMock.Setup(r => r.GetByIdAsync(conjunction.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conjunction);

        var result = await CreateHandler().HandleAsync(
            new DeleteConjunctionEventCommand(conjunction.Id), CancellationToken.None);

        result.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(conjunction.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NonExistentEvent_ThrowsConjunctionEventNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConjunctionEvent?)null);

        var act = async () => await CreateHandler().HandleAsync(
            new DeleteConjunctionEventCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<ConjunctionEventNotFoundException>();
    }
}
