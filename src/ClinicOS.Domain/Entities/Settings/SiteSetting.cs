namespace ClinicOS.Domain.Entities.Settings;

using ClinicOS.Domain.Common.Entities;

public class SiteSetting : AuditableEntity
{
    public string ClinicName { get; set; } = "ClinicOS";
    public string ContactEmail { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Currency { get; set; } = "EGP";
    public int AppointmentDurationMinutes { get; set; } = 30;
    
    // UI Settings
    public string ThemeColor { get; set; } = "#1E88E5";
    public string? LogoUrl { get; set; }
}
