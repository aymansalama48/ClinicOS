using ClinicOS.Domain.Entities.MedicalRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.ToTable("MedicalRecords");

        builder.Property(mr => mr.Diagnosis).HasMaxLength(2000);
        builder.Property(mr => mr.Notes).HasMaxLength(2000);

        builder.HasIndex(mr => mr.AppointmentId)
            .IsUnique()
            .HasDatabaseName("IX_MedicalRecords_AppointmentId");

        builder.HasMany(mr => mr.Prescriptions)
            .WithOne(p => p.MedicalRecord)
            .HasForeignKey(p => p.MedicalRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(mr => mr.Tests)
            .WithOne(t => t.MedicalRecord)
            .HasForeignKey(t => t.MedicalRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}