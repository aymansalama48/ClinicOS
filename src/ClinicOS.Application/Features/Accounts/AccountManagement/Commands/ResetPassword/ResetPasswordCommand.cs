using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword) : ICommand;