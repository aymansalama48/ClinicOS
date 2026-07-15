using ClinicOS.Domain.Entities.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class DoctorAvailabilityConfiguration : IEntityTypeConfiguration<DoctorAvailability>
{
    public void Configure(EntityTypeBuilder<DoctorAvailability> builder)
    {
        builder.ToTable("DoctorAvailabilities");

        builder.Property(da => da.StartTime).IsRequired();
        builder.Property(da => da.EndTime).IsRequired();
        builder.Property(da => da.MaxPatients).IsRequired();

        builder.HasIndex(da => new { da.DoctorId, da.DayOfWeek, da.Period })
            .IsUnique()
            .HasDatabaseName("IX_DoctorAvailabilities_Doctor_Day_Period");
    }
}