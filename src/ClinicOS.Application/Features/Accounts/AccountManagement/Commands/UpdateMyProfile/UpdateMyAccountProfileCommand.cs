using ClinicOS.Application.Common.Abstractions.External.Cache;
using ClinicOS.Application.Common.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.UpdateMyProfile
{
    public sealed record UpdateMyAccountProfileCommand(
        string FirstName,
        string? MiddleName,
        string LastName,
        string? PhoneNumber
    ) : ICommand<bool>, ICacheInvalidatorCommand
    {
        // هيمسح كاش شاشة الـ CRM عشان لو الآدمن فاتح اللستة يشوف اسمه أو رقمه الجديد
        public IReadOnlyCollection<string> CacheKeys => ["users-list"];
    }
}