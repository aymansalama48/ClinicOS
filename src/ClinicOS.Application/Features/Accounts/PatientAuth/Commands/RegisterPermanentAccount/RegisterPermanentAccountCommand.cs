namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.RegisterPermanentAccount;

using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;

public sealed record RegisterPermanentAccountCommand(
    string Email,
    string Password,
    string PhoneNumber,
    string OtpCode) : ICommand<PatientAuthResponse>;