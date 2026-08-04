using ClinicOS.Domain.Entities.Specializations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.ToTable("Specializations");

        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.IsActive).HasDefaultValue(true);

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("IX_Specializations_Name")
            .HasFilter("[IsDeleted] = 0");

        // ربط علاقة الصورة (الأيقونة) بجدول Attachments
        builder.HasOne(s => s.IconAttachment)
            .WithMany()
            .HasForeignKey(s => s.IconAttachmentId)
            .OnDelete(DeleteBehavior.SetNull);

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