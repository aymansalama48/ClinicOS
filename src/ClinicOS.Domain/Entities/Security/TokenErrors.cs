using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Domain.Entities.Security
{
    /// <summary>
    /// أخطاء خاصة بـ Refresh Token للموظفين
    /// </summary>
    public static class TokenErrors
    {
        /// <summary>
        /// Refresh Token غير صالح أو غير موجود
        /// </summary>
        public static readonly Error InvalidRefreshToken = new(
            "Token.InvalidRefreshToken",
            "Refresh Token غير صالح أو غير موجود",
            ErrorType.Validation);

        /// <summary>
        /// Refresh Token ملغي (تم تسجيل الخروج منه)
        /// </summary>
        public static readonly Error TokenRevoked = new(
            "Token.Revoked",
            "Refresh Token ملغي، يرجى تسجيل الدخول مجدداً",
            ErrorType.Validation);

        /// <summary>
        /// انتهت صلاحية Refresh Token
        /// </summary>
        public static readonly Error TokenExpired = new(
            "Token.Expired",
            "انتهت صلاحية Refresh Token، يرجى تسجيل الدخول مجدداً",
            ErrorType.Validation);

        /// <summary>
        /// فشل في إنشاء Refresh Token بسبب خطأ غير متوقع
        /// </summary>
        public static readonly Error TokenGenerationFailed = new(
            "Token.GenerationFailed",
            "حدث خطأ أثناء إنشاء Refresh Token",
            ErrorType.Failure);
    }
}
