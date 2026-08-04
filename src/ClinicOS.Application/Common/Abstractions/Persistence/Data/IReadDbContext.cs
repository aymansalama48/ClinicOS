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
using ClinicOS.Domain.Entities.Settings;
using ClinicOS.Domain.Entities.Specializations;
using ClinicOS.Domain.Entities.Storage;
using System.Linq;

namespace ClinicOS.Application.Common.Abstractions.Persistence.Data
{
    public interface IReadDbContext
    {
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
        IQueryable<Notification> Notifications { get; }
        IQueryable<EmailLog> EmailLogs { get; }
        IQueryable<Attachment> Attachments { get; }
        IQueryable<SiteSetting> SiteSettings { get; }
    }
}
