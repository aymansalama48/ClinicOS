using System;
using ClinicOS.Domain.Common.Events;

namespace ClinicOS.Domain.Entities.Appointments.Events;

/// <summary>
/// حدث نطاق (Domain Event) ينطلق عند تأكيد الموعد
/// </summary>
public record AppointmentConfirmedEvent(Guid AppointmentId) : IDomainEvent;
