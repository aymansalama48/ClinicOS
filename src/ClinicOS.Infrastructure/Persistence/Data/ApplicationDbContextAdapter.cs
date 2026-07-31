namespace ClinicOS.Infrastructure.Persistence.Data;

using ClinicOS.Application.Common.Abstractions.Persistence;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// هذا الكلاس يعمل كمترجم (Adapter) بين قاعدة بيانات EF Core وبين طبقة الـ Application.
/// يخفي تفاصيل الـ DbSet ويوفر فقط IQueryable.
/// </summary>
public class ApplicationDbContextAdapter(AppDbContext context) : IApplicationDbContext
{
    // ==============================
    // 1. القراءة (IQueryable Mapping)
    // ==============================
    public IQueryable<Specialization> Specializations => context.Specializations;
    public IQueryable<SpecializationSchedule> SpecializationSchedules => context.SpecializationSchedules;
    public IQueryable<Doctor> Doctors => context.Doctors;
    public IQueryable<DoctorAvailability> DoctorAvailabilities => context.DoctorAvailabilities;
    public IQueryable<Receptionist> Receptionists => context.Receptionists;
    public IQueryable<Patient> Patients => context.Patients;
    public IQueryable<Appointment> Appointments => context.Appointments;
    public IQueryable<Payment> Payments => context.Payments;
    public IQueryable<MedicalRecord> MedicalRecords => context.MedicalRecords;
    public IQueryable<Prescription> Prescriptions => context.Prescriptions;
    public IQueryable<PrescriptionItem> PrescriptionItems => context.PrescriptionItems;
    public IQueryable<Test> Tests => context.Tests;
    public IQueryable<OtpVerification> OtpVerifications => context.OtpVerifications;
    public IQueryable<AuditLog> AuditLogs => context.AuditLogs;
    public IQueryable<StaffInvitation> StaffInvitations => context.StaffInvitations;

    // ==============================
    // عمليات الكتابة (عنصر واحد)
    // ==============================
    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        context.Set<TEntity>().Add(entity);
    }

    public void Update<TEntity>(TEntity entity) where TEntity : class
    {
        context.Set<TEntity>().Update(entity);
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : class
    {
        context.Set<TEntity>().Remove(entity);
    }

    // ==============================
    // 👇 عمليات الكتابة (مجموعة Range)
    // ==============================
    public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        context.Set<TEntity>().AddRange(entities);
    }

    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        context.Set<TEntity>().UpdateRange(entities);
    }

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        context.Set<TEntity>().RemoveRange(entities);
    }
    // ==============================
    // 3. حفظ التغييرات
    // ==============================
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}