using System;
using ClinicOS.Domain.Common.Events;

namespace ClinicOS.Domain.Entities.Appointments.Events;

/// <summary>
/// حدث نطاق (Domain Event) ينطلق عند اكتمال الكشف
/// </summary>
public record AppointmentCompletedEvent(Guid AppointmentId) : IDomainEvent;
