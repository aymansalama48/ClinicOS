using ClinicOS.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Domain.Entities.Doctors.Events
{
    // حدث تعديل أسعار الكشف
    public record DoctorFeesUpdatedDomainEvent(
        Guid DoctorId,
        decimal NewConsultationFee,
        decimal NewUrgentSurchargeFee) : IDomainEvent;
}
