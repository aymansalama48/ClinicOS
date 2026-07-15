using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.Property(d => d.Bio).HasMaxLength(1000);
        builder.Property(d => d.ConsultationFee).HasPrecision(18, 2).IsRequired();
        builder.Property(d => d.UrgentSurchargeFee).HasPrecision(18, 2).IsRequired();

        // ApplicationUserId فريد (يمنع ربط حساب واحد بأكثر من طبيب)
        builder.HasIndex(d => d.ApplicationUserId)
            .IsUnique()
            .HasDatabaseName("IX_Doctors_ApplicationUserId");


        // العلاقة مع ApplicationUser (1:1)
        builder.HasOne<ApplicationUser>()
            .WithOne(u => u.Doctor)
            .HasForeignKey<Doctor>(d => d.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // العلاقات مع الكيانات الأخرى
        builder.HasMany(d => d.Availabilities)
            .WithOne(da => da.Doctor)
            .HasForeignKey(da => da.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Appointments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}