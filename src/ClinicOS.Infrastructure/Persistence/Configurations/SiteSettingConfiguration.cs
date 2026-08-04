namespace ClinicOS.Infrastructure.Persistence.Configurations;

using ClinicOS.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
{
    public void Configure(EntityTypeBuilder<SiteSetting> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ClinicName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ContactEmail).HasMaxLength(150);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Currency).IsRequired().HasMaxLength(10);
        builder.Property(x => x.ThemeColor).HasMaxLength(20);
        builder.Property(x => x.LogoUrl).HasMaxLength(1000);
        
        // Seed default settings
        builder.HasData(new SiteSetting
        {
            Id = System.Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ClinicName = "ClinicOS",
            Currency = "EGP",
            AppointmentDurationMinutes = 30,
            ThemeColor = "#1E88E5",
            CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
        });
    }
}
