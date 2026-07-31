using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Invitation;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Entities.OtpVerification;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.Prescriptions;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Domain.Entities.Specializations;
using System.Linq; // 👈 استخدمنا دي بدل EF Core
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Common.Abstractions.Persistence
{
    public interface IApplicationDbContext
    {
        // 1. عمليات القراءة (استخدمنا IQueryable المستقلة بدلاً من DbSet)
        IQueryable<Specialization> Specializations { get; }
        IQueryable<SpecializationSchedule> SpecializationSchedules { get; }

        IQueryable<Doctor> Doctors { get; }
        IQueryable<DoctorAvailability> DoctorAvailabilities { get; }

        IQueryable<Receptionist> Receptionists { get; }

        IQueryable<Patient> Patients { get; }

        IQueryable<Appointment> Appointments { get; }
        IQueryable<Payment> Payments { get; }

        IQueryable<MedicalRecord> MedicalRecords { get; }
        IQueryable<Prescription> Prescriptions { get; }
        IQueryable<PrescriptionItem> PrescriptionItems { get; }
        IQueryable<Test> Tests { get; }

        IQueryable<OtpVerification> OtpVerifications { get; }
        IQueryable<AuditLog> AuditLogs { get; }
        IQueryable<StaffInvitation> StaffInvitations { get; }

        // 2. عمليات الكتابة والإضافة والحذف (Generic Methods)
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;

        // 👇 عمليات المجموعة (Range Operations)
        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        // 3. حفظ التغييرات
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}