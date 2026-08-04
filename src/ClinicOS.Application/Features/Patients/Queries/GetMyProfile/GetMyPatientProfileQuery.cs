using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Patients.Queries.GetMyProfile;

public sealed record GetMyPatientProfileQuery() : IQuery<PatientProfileResponse>;