using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.DeactivateUser;

// بياخد الـ ID بتاع اليوزر اللي الآدمن عاوز يوقفه
public sealed record DeactivateUserCommand(Guid UserId) : ICommand<bool>, ICacheInvalidatorCommand
{
    // هيمسح كاش شاشة الـ CRM عشان حالة اليوزر اتغيرت
    public IReadOnlyCollection<string> CacheKeys => ["users-list"];
}