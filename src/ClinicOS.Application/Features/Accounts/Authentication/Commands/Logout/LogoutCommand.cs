using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Accounts.Authentication.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken) : ICommand<bool>;