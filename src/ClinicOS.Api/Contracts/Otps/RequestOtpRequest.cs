using ClinicOS.Domain.Enums;

namespace ClinicOS.Api.Contracts.Otps;

public sealed record RequestOtpRequest(
    string PhoneNumber,
    OtpPurpose Purpose,
    Guid? AppointmentId = null);
