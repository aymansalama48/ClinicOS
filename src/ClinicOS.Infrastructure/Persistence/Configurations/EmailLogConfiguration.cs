namespace ClinicOS.Infrastructure.Persistence.Configurations;
using ClinicOS.Domain.Entities.Emails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.To).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Subject).IsRequired().HasMaxLength(255);
    }
}
