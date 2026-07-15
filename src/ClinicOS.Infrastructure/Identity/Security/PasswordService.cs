using ClinicOS.Application.Common.Abstractions.Identity.Security;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Security;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ClinicOS.Infrastructure.Identity.Security;

public class PasswordService : IPasswordService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public PasswordService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(PasswordErrors.UserNotFound);

        // UserManager بتاع Identity مش بياخد CancellationToken في الـ Methods بتاعته أصلاً (قيد في المكتبة نفسها)
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            // لو الغلط تحديدًا "الباسورد الحالي غلط" بنرجّع Error مخصوص، غير كده Error عام
            var isWrongCurrentPassword = result.Errors
                .Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordMismatch));

            return Result.Failure(isWrongCurrentPassword
                ? PasswordErrors.IncorrectCurrentPassword
                : PasswordErrors.ChangeFailed);
        }

        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        // ملحوظة أمان مهمة: برضه بترجع Success حتى لو المستخدم مش موجود
        // عشان محدش يقدر يستخدم الـ Endpoint ده "يتحقق" إن إيميل معين مسجل عندنا ولا لأ (Email Enumeration)
        if (user is null)
            return Result.Success();

        var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // التوكن اللي Identity بترجّعه فيه رموز خاصة، لازم ترمّزه لـ URL-Safe قبل ما تحطه في اللينك
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

        // TODO: تبعت اللينك ده عن طريق IEmailSender بتاعتك (مش موجودة عندي هنا)
        // مثال: await _emailSender.SendPasswordResetEmailAsync(email, encodedToken, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(PasswordErrors.ResetFailed); // نفس رسالة عامة، متقولش "الإيميل غلط" تحديدًا

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch
        {
            return Result.Failure(PasswordErrors.ResetFailed);
        }

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(PasswordErrors.ResetFailed);
    }
}