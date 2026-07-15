using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.Identity.Security
{
    /// <summary>
    /// مسئول عن جميع العمليات المتعلقة بكلمات المرور.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// تغيير كلمة المرور للمستخدم الحالي.
        /// </summary>
        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);


        /// <summary>
        /// إرسال رابط إعادة تعيين كلمة المرور إلى البريد الإلكتروني.
        /// </summary>
        Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken);


        /// <summary>
        /// إعادة تعيين كلمة المرور باستخدام الـ Token.
        /// </summary>
        Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);

    }
}
