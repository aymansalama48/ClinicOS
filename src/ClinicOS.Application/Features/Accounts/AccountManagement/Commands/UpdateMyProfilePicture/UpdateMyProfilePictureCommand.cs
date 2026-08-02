using ClinicOS.Application.Common.Abstractions.Messaging;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.UpdateMyProfilePicture;

public sealed record UpdateMyProfilePictureCommand(string AvatarUrl) : ICommand<bool>;