using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;

namespace ClinicOS.Application.Features.Accounts.Authentication.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken) : ICommand<StaffAuthResponse>;