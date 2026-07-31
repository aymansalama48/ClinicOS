// ClinicOS.Application/Common/Events/DomainEventNotification.cs
namespace ClinicOS.Application.Common.Events;

using MediatR;
using ClinicOS.Domain.Common.Events;

public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent)
    : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}