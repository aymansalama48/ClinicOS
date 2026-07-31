namespace ClinicOS.Application.Features.Accounts.StaffAuth.Commands.StaffGoogleLogin;

using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.StaffAuth.Shared;

public sealed record StaffGoogleLoginCommand(string IdToken) : ICommand<StaffAuthResponse>;