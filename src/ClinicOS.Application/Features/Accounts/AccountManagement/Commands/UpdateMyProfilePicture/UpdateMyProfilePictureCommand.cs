using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Messaging;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.UpdateMyProfilePicture;

public sealed record UpdateMyProfilePictureCommand(string AvatarUrl) : ICommand<bool>, ICacheInvalidatorCommand
{
    // هيمسح كاش شاشة الـ CRM عشان صورته الجديدة تظهر للآدمن
    public IReadOnlyCollection<string> CacheKeys => ["users-list"];
}