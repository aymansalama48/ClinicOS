using System;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetReceptionistById;

public sealed record ReceptionistDetailsResponse(
    Guid Id,
    Guid SpecializationId,
    Guid ApplicationUserId,
    bool IsActive
);