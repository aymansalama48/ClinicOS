using ClinicOS.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Domain.Entities.Doctors.Events
{
    // حدث إنشاء الدكتور
    public record DoctorCreatedDomainEvent(
        Guid DoctorId,
        Guid ApplicationUserId,
        Guid SpecializationId) : IDomainEvent;
}
