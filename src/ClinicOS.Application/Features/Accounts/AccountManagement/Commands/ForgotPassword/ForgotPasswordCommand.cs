using ClinicOS.Application.Common.Abstractions.Messaging;
using MediatR;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand;