using ClinicOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicOS.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.Property(al => al.ActionType).HasMaxLength(100).IsRequired();
        builder.Property(al => al.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(al => al.EntityId).HasMaxLength(50).IsRequired();
        builder.Property(al => al.AdditionalInfo).HasMaxLength(500);
        builder.Property(al => al.IpAddress).HasMaxLength(45);

        builder.HasIndex(al => al.EntityName)
            .HasDatabaseName("IX_AuditLogs_EntityName");

        builder.HasIndex(al => al.EntityId)
            .HasDatabaseName("IX_AuditLogs_EntityId");

        builder.HasIndex(al => al.CreatedAt)
            .HasDatabaseName("IX_AuditLogs_CreatedAt");
    }
}