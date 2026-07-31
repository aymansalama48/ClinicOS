using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Accounts.PatientAuth.Shared
{
    public record PatientAuthResponse(
        Guid PatientId,
        string AccessToken,
        int ExpiresInSeconds,
        bool IsPermanentAccount,
        bool RequiresPhoneNumber = false);
}
