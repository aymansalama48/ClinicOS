using ClinicOS.Domain.Entities.Prescriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
{
    public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
    {
        builder.ToTable("PrescriptionItems");

        builder.Property(pi => pi.MedicineName).HasMaxLength(200).IsRequired();
        builder.Property(pi => pi.Dosage).HasMaxLength(100);
        builder.Property(pi => pi.Frequency).HasMaxLength(100);
        builder.Property(pi => pi.Duration).HasMaxLength(100);
        builder.Property(pi => pi.Instructions).HasMaxLength(500);
    }
}