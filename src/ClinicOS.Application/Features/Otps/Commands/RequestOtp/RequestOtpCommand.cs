namespace ClinicOS.Application.Features.Otps.Commands.RequestOtp;

using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Domain.Enums;

public sealed record RequestOtpCommand(
    string PhoneNumber,
    OtpPurpose Purpose,
    Guid? AppointmentId = null) : ICommand<RequestOtpResponse>;