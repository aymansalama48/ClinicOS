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

        // 👇 التعديل هنا: شلنا IsRequired لأن العدد ممكن يكون Null (مفتوح)
        builder.Property(da => da.MaxPatients).IsRequired(false);

        // الفهرس العبقري بتاعك (يمنع تكرار نفس الفترة في نفس اليوم لنفس الدكتور)
        builder.HasIndex(da => new { da.DoctorId, da.DayOfWeek, da.Period })
            .IsUnique()
            .HasDatabaseName("IX_DoctorAvailabilities_Doctor_Day_Period");

        // 👇 تأكيد العلاقة (Relationships) لضمان سلامة البيانات
        builder.HasOne(da => da.Doctor)
            .WithMany() // لو عندك List<DoctorAvailability> جوه كيان الدكتور، اكتبها هنا
            .HasForeignKey(da => da.DoctorId)
            .OnDelete(DeleteBehavior.Cascade); // لو الدكتور اتمسح، مواعيده تتمسح معاه
    }
}