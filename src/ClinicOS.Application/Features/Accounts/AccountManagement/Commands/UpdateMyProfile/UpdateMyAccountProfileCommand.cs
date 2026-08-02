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
    ) : ICommand<bool>;
}
