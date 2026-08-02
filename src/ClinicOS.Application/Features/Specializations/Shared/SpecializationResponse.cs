using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Specializations.Shared
{
    // DTO لقائمة التخصصات
    public record SpecializationResponse(
        Guid Id,
        string Name,
        string? Description
    );
}
