using ClinicOS.Application.Common.Abstractions.Identity.Authentication;
using ClinicOS.Application.Features.Accounts.Shared;
using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.Identity.Tokens
{
    public interface IRefreshTokenService
    {
        /// <summary>
        /// إنشاء وتخزين Refresh Token جديد للموظف
        /// </summary>
        Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);


        /// <summary>
        /// تجديد الـ Access Token باستخدام Refresh Token صالح
        /// </summary>
        Task<Result<StaffAuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);


        /// <summary>
        /// إلغاء صلاحية Refresh Token معين
        /// </summary>
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);


        /// <summary>
        /// إلغاء كافة جلسات المستخدم عبر جميع الأجهزة والشركات (Global Revoke)
        /// </summary>
        Task<Result> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken);
    }
}
