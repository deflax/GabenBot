




public abstract class AggregateRoot
{
    private List<IDomainEvent> _domainEvents;
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent newEvent)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(newEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}
