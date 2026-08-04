using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Domain.Entities.Specializations;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Entities.AuditLogs;
using ClinicOS.Domain.Entities.Invitation;
using ClinicOS.Domain.Entities.Notifications;
using ClinicOS.Domain.Entities.OtpVerification;
using ClinicOS.Domain.Entities.Emails;
using ClinicOS.Domain.Entities.Storage;
using ClinicOS.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Common.Abstractions.Persistence.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Appointment> Appointments { get; }
        DbSet<MedicalRecord> MedicalRecords { get; }
        DbSet<Doctor> Doctors { get; }
        DbSet<DoctorAvailability> DoctorAvailabilities { get; }
        DbSet<Patient> Patients { get; }
        DbSet<Receptionist> Receptionists { get; }
        DbSet<Specialization> Specializations { get; }
        DbSet<SpecializationSchedule> SpecializationSchedules { get; }
        DbSet<StaffInvitation> StaffInvitations { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<OtpVerification> OtpVerifications { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<EmailLog> EmailLogs { get; }
        DbSet<Attachment> Attachments { get; }
        DbSet<SiteSetting> SiteSettings { get; }

       // EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
       // EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
       // EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
