using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Entities.OtpVerification;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.Prescriptions;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Domain.Entities.Specializations;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Application.Common.Abstractions.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<Specialization> Specializations { get; }
        DbSet<SpecializationSchedule> SpecializationSchedules { get; }

        DbSet<Doctor> Doctors { get; }
        DbSet<DoctorAvailability> DoctorAvailabilities { get; }

        DbSet<Receptionist> Receptionists { get; }

        DbSet<Patient> Patients { get; }

        DbSet<Appointment> Appointments { get; }
        DbSet<Payment> Payments { get; }

        DbSet<MedicalRecord> MedicalRecords { get; }
        DbSet<Prescription> Prescriptions { get; }
        DbSet<PrescriptionItem> PrescriptionItems { get; }
        DbSet<Test> Tests { get; }

        DbSet<OtpVerification> OtpVerifications { get; }
        DbSet<AuditLog> AuditLogs { get; }

      //  DbSet<RefreshToken> RefreshTokens { get; }
     //   DbSet<OutboxMessage> OutboxMessages { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}