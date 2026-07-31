namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.PatientOtpLogin;

using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;

public sealed class PatientOtpLoginCommandHandler(
    IPatientAuthService patientAuthService) : ICommandHandler<PatientOtpLoginCommand, PatientAuthResponse>
{
    public async Task<Result<PatientAuthResponse>> Handle(
        PatientOtpLoginCommand request,
        CancellationToken cancellationToken)
    {
        return await patientAuthService.LoginWithOtpAsync(
            request.PhoneNumber,
            request.Code,
            request.Purpose,
            cancellationToken);
    }
}