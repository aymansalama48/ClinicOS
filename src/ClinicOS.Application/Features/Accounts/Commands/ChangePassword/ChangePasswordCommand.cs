using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Accounts.Commands.ChangePassword;

/// <summary>
/// أمر تغيير كلمة المرور للمستخدم المسجل حالياً
/// </summary>
public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword) : ICommand;