using ClinicOS.Domain.Entities.Invitation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Infrastructure.Persistence.Configurations
{
    public sealed class StaffInvitationConfiguration : IEntityTypeConfiguration<StaffInvitation>
    {
        public void Configure(EntityTypeBuilder<StaffInvitation> builder)
        {
            // اسم الجدول
            builder.ToTable("StaffInvitations");

            // المفتاح الأساسي (لو لم يتم تعريفه في الـ AuditableEntity)
            builder.HasKey(x => x.Id);

            // الحقول الإجبارية وأطوالها
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.AdminName)
                .IsRequired()
                .HasMaxLength(150);

            // الفهارس (Indexes) لتحسين أداء البحث (الاستعلامات تتم غالباً عبر الـ Token أو Email)
            builder.HasIndex(x => x.Token).IsUnique(); // التوكن يجب أن يكون فريداً
            builder.HasIndex(x => x.Email);

            // العلاقات (إن وُجدت):
            // إذا كان لديك كيان Specialization وتريد ربطه صراحةً، يمكنك إضافة:
            // builder.HasOne<Specialization>()
            //        .WithMany()
            //        .HasForeignKey(x => x.SpecializationId)
            //        .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
