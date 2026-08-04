namespace ClinicOS.Infrastructure.Options;

public class GoogleDriveOptions
{
    public const string SectionName = "GoogleDrive";
    public string RootFolderId { get; set; } = string.Empty;
    public string ServiceAccountFilePath { get; set; } = string.Empty;
    public long MaxFileSizeBytes { get; set; }
    public string[] AllowedMimeTypes { get; set; } = [];
    public double QuotaWarningThreshold { get; set; }
}
