using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.PaymentMethod).HasMaxLength(50);

        builder.HasIndex(p => p.AppointmentId)
            .HasDatabaseName("IX_Payments_AppointmentId");
    }
}