using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.Property(a => a.BookingName).HasMaxLength(200);
        builder.Property(a => a.BookingPhone).HasMaxLength(20);
        builder.Property(a => a.TotalAmount).HasPrecision(18, 2);
        builder.Property(a => a.FollowUpFee).HasPrecision(18, 2);
        builder.Property(a => a.Notes).HasMaxLength(500);

        // RowVersion للتزامن
        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        // الفهارس
        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.Period })
            .HasDatabaseName("IX_Appointments_Doctor_Date_Period");

        builder.HasIndex(a => a.PatientId)
            .HasDatabaseName("IX_Appointments_PatientId");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_Appointments_Status");

        builder.HasIndex(a => a.QueuedAt)
            .HasDatabaseName("IX_Appointments_QueuedAt");

        // فريد مرشح لرقم الطابور (يمنع التكرار داخل نفس السياق)
        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.Period, a.QueueNumber })
            .IsUnique()
            .HasDatabaseName("IX_Appointments_Queue_Unique")
            .HasFilter("[QueueNumber] IS NOT NULL");

        // العلاقات
        builder.HasOne(a => a.MedicalRecord)
            .WithOne(mr => mr.Appointment)
            .HasForeignKey<MedicalRecord>(mr => mr.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Payments)
            .WithOne(p => p.Appointment)
            .HasForeignKey(p => p.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}