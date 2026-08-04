using System.Collections.Generic;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Models;

namespace ClinicOS.Application.Features.Doctors.Queries.GetDoctorsLookup;

/// <summary>
/// استعلام للحصول على قائمة مبسطة بالأطباء (معرف واسم فقط) لتعبئة القوائم المنسدلة
/// </summary>
public record GetDoctorsLookupQuery(string? SearchTerm = null) : IQuery<List<LookupResponse>>;
