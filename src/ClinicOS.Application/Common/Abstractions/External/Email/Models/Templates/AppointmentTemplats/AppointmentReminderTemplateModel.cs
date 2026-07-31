using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.AppointmentTemplats;

/// <summary>
/// نموذج بريد تذكير الموعد
/// </summary>
public class AppointmentReminderTemplateModel : BaseEmailTemplateModel
{
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string AppointmentDate { get; set; } = string.Empty;
    public string AppointmentTime { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}