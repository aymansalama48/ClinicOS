namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientOtpLogin;

using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Enums;

public sealed record PatientOtpLoginCommand(
    string PhoneNumber,
    string Code,
    OtpPurpose Purpose) : ICommand<PatientAuthResponse>;