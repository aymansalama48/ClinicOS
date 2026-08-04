namespace ClinicOS.Infrastructure.Persistence.Data;

using ClinicOS.Application.Common.Abstractions.Persistence.Data;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.AuditLogs;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Emails;
using ClinicOS.Domain.Entities.Invitation;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Entities.Notifications;
using ClinicOS.Domain.Entities.OtpVerification;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.Prescriptions;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Domain.Entities.Specializations;
using ClinicOS.Domain.Entities.Storage;
using ClinicOS.Domain.Entities.Settings;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using ClinicOS.Infrastructure.Persistence.Outbox;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext, IReadDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
    }

    // ==============================
    // DbSets الخاصة بـ Entity Framework
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
    public DbSet<StaffInvitation> StaffInvitations => Set<StaffInvitation>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

    // ==============================
    // تطبيق IReadDbContext لـ القراءة فقط (AsNoTracking)
    // ==============================
    IQueryable<Specialization> IReadDbContext.Specializations => Specializations.AsNoTracking();
    IQueryable<SpecializationSchedule> IReadDbContext.SpecializationSchedules => SpecializationSchedules.AsNoTracking();
    IQueryable<Doctor> IReadDbContext.Doctors => Doctors.AsNoTracking();
    IQueryable<DoctorAvailability> IReadDbContext.DoctorAvailabilities => DoctorAvailabilities.AsNoTracking();
    IQueryable<Receptionist> IReadDbContext.Receptionists => Receptionists.AsNoTracking();
    IQueryable<Patient> IReadDbContext.Patients => Patients.AsNoTracking();
    IQueryable<Appointment> IReadDbContext.Appointments => Appointments.AsNoTracking();
    IQueryable<Payment> IReadDbContext.Payments => Payments.AsNoTracking();
    IQueryable<MedicalRecord> IReadDbContext.MedicalRecords => MedicalRecords.AsNoTracking();
    IQueryable<Prescription> IReadDbContext.Prescriptions => Prescriptions.AsNoTracking();
    IQueryable<PrescriptionItem> IReadDbContext.PrescriptionItems => PrescriptionItems.AsNoTracking();
    IQueryable<Test> IReadDbContext.Tests => Tests.AsNoTracking();
    IQueryable<OtpVerification> IReadDbContext.OtpVerifications => OtpVerifications.AsNoTracking();
    IQueryable<AuditLog> IReadDbContext.AuditLogs => AuditLogs.AsNoTracking();
    IQueryable<StaffInvitation> IReadDbContext.StaffInvitations => StaffInvitations.AsNoTracking();
    IQueryable<Notification> IReadDbContext.Notifications => Notifications.AsNoTracking();
    IQueryable<EmailLog> IReadDbContext.EmailLogs => EmailLogs.AsNoTracking();
    IQueryable<Attachment> IReadDbContext.Attachments => Attachments.AsNoTracking();
    IQueryable<SiteSetting> IReadDbContext.SiteSettings => SiteSettings.AsNoTracking();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. تطبيق كل إعدادات الجداول من الـ Configurations
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 2. تطبيق الفلتر العام للـ Soft Delete (باستخدام الدالة اللي عملناها) 👇
        builder.ApplySoftDeleteGlobalFilters();
    }
}
