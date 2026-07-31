namespace ClinicOS.Application.Common.Abstractions.External.Email.Constants;

/// <summary>
/// أسماء قوالب البريد الإلكتروني المستخدمة في النظام
/// </summary>
public static class EmailTemplateNames
{
    public const string Login = "Login";
    public const string AccountLocked = "AccountLocked";
    public const string AccountUnlocked = "AccountUnlocked";
    public const string EmailConfirmation = "EmailConfirmation";
    public const string PasswordChanged = "PasswordChanged";
    public const string ResetPassword = "ResetPassword";
    public const string Welcome = "Welcome";
    public const string RoleAssigned = "RoleAssigned";
    public const string RoleRemoved = "RoleRemoved";
    public const string AppointmentConfirmation = "AppointmentConfirmation";
    public const string AppointmentReminder = "AppointmentReminder";
}