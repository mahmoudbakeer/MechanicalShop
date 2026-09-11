using System.ComponentModel.DataAnnotations.Schema;

namespace MechanicShop.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; }

    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    protected Entity() { }

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
    }

    public void AddDomainEvent(DomainEvent Event)
    {
        _domainEvents.Add(Event);
    }

    public void DeleteDomainEvent(DomainEvent Event)
    {
        _domainEvents.Remove(Event);
    }

    public void ClearDomainEvent()
    {
        _domainEvents.Clear();
    }
}

