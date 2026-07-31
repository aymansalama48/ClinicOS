using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;

namespace ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffLogin;

public sealed record StaffLoginCommand(
    string Email,
    string Password) : ICommand<StaffAuthResponse>;