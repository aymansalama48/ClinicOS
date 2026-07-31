namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientGoogleLogin;

using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;

public sealed class PatientGoogleLoginCommandHandler(
    IPatientAuthService patientAuthService) : ICommandHandler<PatientGoogleLoginCommand, PatientAuthResponse>
{
    public async Task<Result<PatientAuthResponse>> Handle(
        PatientGoogleLoginCommand request,
        CancellationToken cancellationToken)
    {
        return await patientAuthService.LoginWithGoogleAsync(
            request.IdToken,
            cancellationToken);
    }
}