using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        // 🛑 استبعاد الخواص المحسوبة في الكود من التخزين في الداتا بيز
    //   builder.Ignore(p => p.FullName);
      //  builder.Ignore(p => p.IsAccountLinked);

        // إعداد باقي الأعمدة الحقيقية
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.MiddleName).HasMaxLength(100);
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();

        builder.Property(p => p.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(p => p.EmergencyContact).HasMaxLength(200);

        // PhoneNumber فريد فقط بين السجلات النشطة
        builder.HasIndex(p => new { p.PhoneNumber, p.IsDeleted })
            .IsUnique()
            .HasDatabaseName("IX_Patients_PhoneNumber_IsDeleted")
            .HasFilter("IsDeleted = 0");

        // ApplicationUserId فريد إذا كان موجوداً
        builder.HasIndex(p => p.ApplicationUserId)
            .IsUnique()
            .HasDatabaseName("IX_Patients_ApplicationUserId")
            .HasFilter("[ApplicationUserId] IS NOT NULL");

        // العلاقة مع ApplicationUser (1:0..1) - اختيارية من الطرفين
        builder.HasOne<ApplicationUser>()
            .WithOne(u => u.Patient)
            .HasForeignKey<Patient>(p => p.ApplicationUserId) // EF Core يستنتج أنها اختيارية لأن ApplicationUserId نوعه Guid?
            .IsRequired(false) // تأكيد إضافي إن العلاقة اختيارية
            .OnDelete(DeleteBehavior.Restrict);

        // العلاقة مع Appointments
        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}