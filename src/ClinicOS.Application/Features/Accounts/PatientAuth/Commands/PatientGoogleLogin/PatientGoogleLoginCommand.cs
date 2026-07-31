namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientGoogleLogin;

using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;

public sealed record PatientGoogleLoginCommand(string IdToken) : ICommand<PatientAuthResponse>;