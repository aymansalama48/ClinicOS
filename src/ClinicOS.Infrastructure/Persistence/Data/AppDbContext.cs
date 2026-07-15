using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.Prescriptions;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Domain.Entities.Security;
using ClinicOS.Domain.Entities.Specializations;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using ClinicOS.Infrastructure.Persistence.Outbox;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClinicOS.Infrastructure.Persistence.Data
{
    /// <summary>
    /// السياق الرئيسي لقاعدة البيانات
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ==============================
        // DbSets للكيانات الأساسية
        // ==============================

        public DbSet<Specialization> Specializations => Set<Specialization>();
        public DbSet<SpecializationSchedule> SpecializationSchedules => Set<SpecializationSchedule>();

        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();

        public DbSet<Receptionist> Receptionists => Set<Receptionist>();

        public DbSet<Patient> Patients => Set<Patient>();

        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
        public DbSet<Test> Tests => Set<Test>();

        public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // تطبيق تكوينات Identity الأساسية
            base.OnModelCreating(builder);

            // تطبيق جميع تكوينات IEntityTypeConfiguration من الـ Assembly الحالي
            // جميع العلاقات والفهارس والقيود محددة في ملفات التكوين المنفصلة
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // لا توجد أي علاقات إضافية هنا - كل شيء في Configurations
        }
    }
}