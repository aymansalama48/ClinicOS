namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientEmailLogin;

using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;

public sealed class PatientEmailLoginCommandHandler(
    IPatientAuthService patientAuthService) : ICommandHandler<PatientEmailLoginCommand, PatientAuthResponse>
{
    public async Task<Result<PatientAuthResponse>> Handle(
        PatientEmailLoginCommand request,
        CancellationToken cancellationToken)
    {
        return await patientAuthService.LoginWithEmailAsync(
            request.Email,
            request.Password,
            cancellationToken);
    }
}