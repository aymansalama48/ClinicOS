namespace ClinicOS.Infrastructure.Persistence.Configurations;

using ClinicOS.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(500);

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);
    }
}
