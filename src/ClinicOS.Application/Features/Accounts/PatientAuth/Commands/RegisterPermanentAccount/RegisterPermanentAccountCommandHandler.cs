namespace ClinicOS.Application.Features.Accounts.PatientAuth.Commands.RegisterPermanentAccount;

using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Features.Accounts.PatientAuth.Shared;
using ClinicOS.Domain.Common.Results;

public sealed class RegisterPermanentAccountCommandHandler(
    IPatientAuthService patientAuthService) : ICommandHandler<RegisterPermanentAccountCommand, PatientAuthResponse>
{
    public async Task<Result<PatientAuthResponse>> Handle(
        RegisterPermanentAccountCommand request,
        CancellationToken cancellationToken)
    {
        return await patientAuthService.RegisterPermanentAccountAsync(
            request.Email,
            request.Password,
            request.PhoneNumber,
            request.OtpCode,
            cancellationToken);
    }
}