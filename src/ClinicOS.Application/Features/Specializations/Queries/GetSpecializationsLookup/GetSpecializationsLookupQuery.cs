using System.Collections.Generic;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Models;

namespace ClinicOS.Application.Features.Specializations.Queries.GetSpecializationsLookup;

/// <summary>
/// استعلام للحصول على قائمة التخصصات (معرف واسم) لتعبئة القوائم المنسدلة في واجهة المستخدم
/// </summary>
public record GetSpecializationsLookupQuery(string? SearchTerm = null) : IQuery<List<LookupResponse>>;
