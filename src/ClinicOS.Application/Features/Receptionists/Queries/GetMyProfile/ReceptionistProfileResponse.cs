using System;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetMyProfile;

public record ReceptionistProfileResponse(
    Guid Id,
    Guid SpecializationId,
    bool IsActive
);