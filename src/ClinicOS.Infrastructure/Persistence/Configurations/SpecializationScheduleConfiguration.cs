using ClinicOS.Domain.Entities.Specializations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class SpecializationScheduleConfiguration : IEntityTypeConfiguration<SpecializationSchedule>
{
    public void Configure(EntityTypeBuilder<SpecializationSchedule> builder)
    {
        builder.ToTable("SpecializationSchedules");

        builder.Property(ss => ss.StartTime).IsRequired();
        builder.Property(ss => ss.EndTime).IsRequired();
        builder.Property(ss => ss.IsActive).HasDefaultValue(true);

        builder.HasIndex(ss => new { ss.SpecializationId, ss.DayOfWeek, ss.Period })
            .IsUnique()
            .HasDatabaseName("IX_SpecializationSchedules_Specialization_Day_Period")
            .HasFilter("[IsDeleted] = 0");
    }
}