using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Aggregates.Conjunctions;

public class ConjunctionEventBuilder : IBuilder<ConjunctionEvent>
{
    private Guid? _primaryObjectId;
    private Guid? _secondaryObjectId;
    private DateTime? _predictedTcaUtc;
    private MissDistance? _missDistance;
    private CollisionProbability? _collisionProbability;

    public ConjunctionEventBuilder WithPrimaryObject(Guid primaryObjectId)
    {
        _primaryObjectId = primaryObjectId;
        return this;
    }

    public ConjunctionEventBuilder WithSecondaryObject(Guid secondaryObjectId)
    {
        _secondaryObjectId = secondaryObjectId;
        return this;
    }

    public ConjunctionEventBuilder WithPredictedTca(DateTime predictedTcaUtc)
    {
        _predictedTcaUtc = predictedTcaUtc;
        return this;
    }

    public ConjunctionEventBuilder WithMissDistance(MissDistance missDistance)
    {
        _missDistance = missDistance;
        return this;
    }

    public ConjunctionEventBuilder WithCollisionProbability(CollisionProbability collisionProbability)
    {
        _collisionProbability = collisionProbability;
        return this;
    }

    /// <summary>Validates all required fields and constructs the ConjunctionEvent.</summary>
    public ConjunctionEvent Build()
    {
        if (_primaryObjectId is null) throw new InvalidOperationException("PrimaryObjectId is required.");
        if (_secondaryObjectId is null) throw new InvalidOperationException("SecondaryObjectId is required.");
        if (_predictedTcaUtc is null) throw new InvalidOperationException("PredictedTcaUtc is required.");
        if (_missDistance is null) throw new InvalidOperationException("MissDistance is required.");
        if (_collisionProbability is null) throw new InvalidOperationException("CollisionProbability is required.");

        return ConjunctionEvent.Create(
            _primaryObjectId.Value,
            _secondaryObjectId.Value,
            _predictedTcaUtc.Value,
            _missDistance,
            _collisionProbability);
    }
}
