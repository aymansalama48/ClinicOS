using System;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetReceptionists;

public sealed record ReceptionistResponse(
    Guid Id,
    Guid SpecializationId,
    Guid ApplicationUserId,
    bool IsActive
);