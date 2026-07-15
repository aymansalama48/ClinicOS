using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class ReceptionistConfiguration : IEntityTypeConfiguration<Receptionist>
{
    public void Configure(EntityTypeBuilder<Receptionist> builder)
    {
        builder.ToTable("Receptionists");

        builder.HasIndex(r => r.ApplicationUserId)
            .IsUnique()
            .HasDatabaseName("IX_Receptionists_ApplicationUserId");


        // العلاقة مع ApplicationUser (1:1)
        builder.HasOne<ApplicationUser>()
            .WithOne(u => u.Receptionist)
            .HasForeignKey<Receptionist>(r => r.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // العلاقة مع Specialization (موجودة في SpecializationConfiguration أيضاً، لكن يمكن وضعها هنا)
        builder.HasOne(r => r.Specialization)
            .WithMany(s => s.Receptionists)
            .HasForeignKey(r => r.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}