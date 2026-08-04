using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(Guid UserId, string RoleName) : ICommand<bool>, ICacheInvalidatorCommand
{
    // هنمسح كاش الآدمن عشان اللستة تتحدث بالرول الجديد فوراً
    public IReadOnlyCollection<string> CacheKeys => ["users-list"];
}