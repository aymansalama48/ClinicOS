using ClinicOS.Domain.Entities.MedicalRecords;
using ClinicOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable("Tests");

        builder.Property(t => t.TestName).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Result).HasMaxLength(2000);
        builder.Property(t => t.ResultFileUrl).HasMaxLength(500);

        builder.HasIndex(t => t.MedicalRecordId)
            .HasDatabaseName("IX_Tests_MedicalRecordId");
    }
}