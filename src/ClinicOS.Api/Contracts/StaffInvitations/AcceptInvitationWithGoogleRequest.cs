namespace ClinicOS.Api.Contracts.StaffInvitations;

public sealed record AcceptInvitationWithGoogleRequest(
    string InvitationToken,
    string GoogleIdToken);
