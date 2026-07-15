using ClinicOS.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Domain.Entities.Doctors.Events
{
    // حدث تعديل بيانات الملف الشخصي
    public record DoctorProfileUpdatedDomainEvent(
        Guid DoctorId,
        string? Bio,
        int YearsOfExperience) : IDomainEvent;
}
