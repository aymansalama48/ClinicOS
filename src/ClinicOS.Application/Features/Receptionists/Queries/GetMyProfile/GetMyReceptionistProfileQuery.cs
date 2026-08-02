using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Receptionists.Queries.GetMyProfile;

public sealed record GetMyReceptionistProfileQuery() : IQuery<ReceptionistProfileResponse>;