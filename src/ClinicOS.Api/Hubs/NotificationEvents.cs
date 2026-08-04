namespace ClinicOS.Api.Hubs;

public static class NotificationEvents
{
    public const string AppointmentConfirmed = "AppointmentConfirmed";
    public const string AppointmentCancelled = "AppointmentCancelled";
    public const string AppointmentReminder = "AppointmentReminder";
    public const string PrescriptionReady = "PrescriptionReady";
    public const string MedicalRecordUpdated = "MedicalRecordUpdated";
    public const string NewMessage = "NewMessage";
    public const string GeneralNotification = "GeneralNotification";
}
