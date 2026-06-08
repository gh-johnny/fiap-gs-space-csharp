namespace OrbitalGuardian.Domain.Shared;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected AggregateRoot() { }

    protected AggregateRoot(TId id) : base(id) { }

    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public IReadOnlyList<IDomainEvent> GetDomainEvents() =>
        _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();
}
