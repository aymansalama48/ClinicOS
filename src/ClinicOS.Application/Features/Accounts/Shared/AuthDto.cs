using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Accounts.Shared
{
    public record AuthDto(
        Guid Id,
        string FullName,
        string Email,
        string Token,
        string RefreshToken // 👈 ضفنا ده عشان يرجع للفرونت إند ويقدر يجدد بيه
    );
}
