using ClinicOS.Domain.Common.Events;

namespace ClinicOS.Domain.Entities.Specializations.Events;

public record SpecializationCreatedDomainEvent(Guid SpecializationId) : IDomainEvent;

public record SpecializationDeactivatedDomainEvent(Guid SpecializationId) : IDomainEvent;

public record SpecializationActivatedDomainEvent(Guid SpecializationId) : IDomainEvent;