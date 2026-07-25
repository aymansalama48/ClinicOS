using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Errors.Identity
{
    public static class ExternalAuthErrors
    {
        public static readonly Error InvalidToken = new(
            "ExternalAuth.InvalidToken",
            "توكن تسجيل الدخول غير صالح أو منتهي الصلاحية.",
            ErrorType.Validation);

        public static readonly Error EmailMissing = new(
            "ExternalAuth.EmailMissing",
            "لا يمكن تسجيل الدخول بدون بريد إلكتروني من مزود الخدمة.",
            ErrorType.Validation);

        public static readonly Error EmailNotVerified = new(
            "ExternalAuth.EmailNotVerified",
            "البريد الإلكتروني غير موثّق من مزود الخدمة.",
            ErrorType.Validation);
    }
}
