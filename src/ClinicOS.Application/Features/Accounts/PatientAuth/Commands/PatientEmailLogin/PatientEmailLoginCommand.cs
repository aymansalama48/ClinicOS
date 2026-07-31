namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientEmailLogin;

using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;

public sealed record PatientEmailLoginCommand(
    string Email,
    string Password) : ICommand<PatientAuthResponse>;