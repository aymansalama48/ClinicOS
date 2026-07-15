using ClinicOS.Domain.Common.Events;

namespace ClinicOS.Domain.Common.Entities;

/// <summary>
/// الكيان الأساسي الذي يدعم أحداث المجال (Domain Events)
/// </summary>
public abstract class Entity : IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}