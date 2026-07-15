using ClinicOS.Domain.Entities.Specializations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.ToTable("Specializations");

        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        // فريد فقط بين السجلات النشطة
        builder.HasIndex(s => new { s.Name, s.IsDeleted })
            .IsUnique()
            .HasDatabaseName("IX_Specializations_Name_IsDeleted")
            .HasFilter("IsDeleted = 0");


        builder.HasMany(s => s.Doctors)
            .WithOne(d => d.Specialization)
            .HasForeignKey(d => d.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Receptionists)
            .WithOne(r => r.Specialization)
            .HasForeignKey(r => r.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Schedules)
            .WithOne(ss => ss.Specialization)
            .HasForeignKey(ss => ss.SpecializationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}