using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetMyProfile;

public sealed record GetMyAccountProfileQuery() : IQuery<MyAccountProfileResponse>;