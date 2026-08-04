using System.Collections.Generic;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Models;

namespace ClinicOS.Application.Features.Patients.Queries.GetPatientsLookup;

/// <summary>
/// استعلام للحصول على قائمة مبسطة بالمرضى (معرف واسم)
/// </summary>
public record GetPatientsLookupQuery(string? SearchTerm = null) : IQuery<List<LookupResponse>>;
