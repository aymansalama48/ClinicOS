using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Accounts.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand;