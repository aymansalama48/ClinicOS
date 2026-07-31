namespace ClinicOS.Infrastructure.Identity.Invitations;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Identity.Invitations;
using ClinicOS.Application.Common.Abstractions.Identity.Providers;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Invitations;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Domain.Constants;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Invitation;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Infrastructure.Persistence.IdentityModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class InvitationService(
    IApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    ICurrentUser currentUser,
    IDateTime dateTime,
    IEnumerable<IExternalAuthProvider> externalAuthProviders,
    ILogger<InvitationService> logger) : IInvitationService
{
    public async Task<Result<string>> SendStaffInvitationAsync(
        string email,
        string role,
        Guid? specializationId = null,
        CancellationToken cancellationToken = default)
    {
        // 1. التحقق من وجود الحساب مسبقاً
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return Result<string>.Failure(UserErrors.EmailAlreadyExists);

        var now = dateTime.Now;

        // 2. إبطال الدعوات المعلقة القديمة لنفس البريد
        var pendingInvitations = await context.StaffInvitations
            .Where(i => i.Email == email && !i.IsUsed && i.ExpiresAtUtc > now)
            .ToListAsync(cancellationToken);

        foreach (var pending in pendingInvitations)
        {
            pending.ExpiresAtUtc = now;
        }

        // 3. إنشاء التوكن والكيان عبر الـ Factory Method (تسجل حدث الإيميل داخلها تلقائياً)
        var token = GenerateSecureToken();
        var invitation = StaffInvitation.Create(
            email: email,
            role: role,
            token: token,
            expiresAtUtc: now.AddDays(2),
            adminId: currentUser.UserId ?? Guid.Empty,
            adminName: currentUser.FullName ?? "System Admin",
            specializationId: specializationId);

        context.Add(invitation);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("تم إنشاء سجل الدعوة بنجاح للبريد {Email}", email);

        return Result<string>.Success(token);
    }

    public async Task<Result<InvitationDetailsDto>> ValidateInvitationTokenAsync(
        string invitationToken,
        CancellationToken cancellationToken = default)
    {
        var invitation = await context.StaffInvitations
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Token == invitationToken, cancellationToken);

        if (invitation is null)
            return Result<InvitationDetailsDto>.Failure(InvitationErrors.NotFound);

        if (invitation.IsUsed)
            return Result<InvitationDetailsDto>.Failure(InvitationErrors.AlreadyUsed);

        if (invitation.ExpiresAtUtc <= dateTime.Now)
            return Result<InvitationDetailsDto>.Failure(InvitationErrors.Expired);

        var details = new InvitationDetailsDto(
            InvitationId: invitation.Id,
            Email: invitation.Email,
            Role: invitation.Role,
            AdminName: invitation.AdminName,
            SpecializationId: invitation.SpecializationId,
            IsValid: true);

        return Result<InvitationDetailsDto>.Success(details);
    }

    public async Task<Result> RevokeInvitationAsync(
        Guid invitationId,
        CancellationToken cancellationToken = default)
    {
        var invitation = await context.StaffInvitations
            .FirstOrDefaultAsync(i => i.Id == invitationId, cancellationToken);

        if (invitation is null)
            return Result.Failure(InvitationErrors.NotFound);

        if (invitation.IsUsed)
            return Result.Failure(InvitationErrors.AlreadyUsed);

        invitation.ExpiresAtUtc = dateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("تم إبطال الدعوة {InvitationId}", invitationId);

        return Result.Success();
    }

    public async Task<Result<bool>> AcceptInvitationAndCreateAccountAsync(
        string invitationToken,
        string fullName,
        string password,
        string? phoneNumber = null,
        CancellationToken cancellationToken = default)
    {
        var invitation = await context.StaffInvitations
            .FirstOrDefaultAsync(i => i.Token == invitationToken, cancellationToken);

        if (invitation is null)
            return Result<bool>.Failure(InvitationErrors.NotFound);

        if (invitation.IsUsed || invitation.ExpiresAtUtc <= dateTime.Now)
            return Result<bool>.Failure(InvitationErrors.InvalidOrExpired);

        // 1. تقسيم الاسم الكامل إلى أجزائه
        var nameParts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : fullName;
        var lastName = nameParts.Length > 1 ? nameParts[^1] : string.Empty;
        var middleName = nameParts.Length > 2 ? string.Join(" ", nameParts[1..^1]) : null;

        // 2. إنشاء المستخدم بالتمرير للخصائص الصحيحة
        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = invitation.Email,
            Email = invitation.Email,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var details = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result<bool>.Failure(UserErrors.CreationFailed(details));
        }

        // إضافة الدور
        var roleResult = await userManager.AddToRoleAsync(user, invitation.Role);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return Result<bool>.Failure(UserErrors.CreationFailed("فشل تعيين الدور الوظيفي للمستخدم."));
        }

        // إنشاء البروفايل الخاص بالدور
        switch (invitation.Role)
        {
            case Roles.Doctor:
                var doctor = new Doctor
                {
                    Id = Guid.CreateVersion7(),
                    ApplicationUserId = user.Id,
                    SpecializationId = invitation.SpecializationId
                        ?? throw new InvalidOperationException("التخصص مطلوب لإنشاء حساب طبيب.")
                };
                context.Add(doctor);
                break;

            case Roles.Receptionist:
                var receptionist = new Receptionist
                {
                    Id = Guid.CreateVersion7(),
                    ApplicationUserId = user.Id,
                    SpecializationId = invitation.SpecializationId
                        ?? throw new InvalidOperationException("التخصص مطلوب لإنشاء حساب وظيفي.")
                };
                context.Add(receptionist);
                break;

            case Roles.Admin:
                break;

            default:
                logger.LogWarning("دور غير معروف: {Role}", invitation.Role);
                break;
        }

        invitation.IsUsed = true;
        invitation.UsedAtUtc = dateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("تم إكمال قبول الدعوة وتفعيل حساب {Email} بالطريقة التقليدية", user.Email);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> AcceptInvitationWithGoogleAsync(
        string invitationToken,
        string googleIdToken,
        CancellationToken cancellationToken = default)
    {
        // 1. التحقق من التوكن الخاص بالدعوة
        var invitation = await context.StaffInvitations
            .FirstOrDefaultAsync(i => i.Token == invitationToken, cancellationToken);

        if (invitation is null)
            return Result<bool>.Failure(InvitationErrors.NotFound);

        if (invitation.IsUsed || invitation.ExpiresAtUtc <= dateTime.Now)
            return Result<bool>.Failure(InvitationErrors.InvalidOrExpired);

        // 2. التحقق من صحة توكن جوجل باستخدام الـ Provider
        var googleProvider = externalAuthProviders.FirstOrDefault(p => p.ProviderName == "Google");
        if (googleProvider is null)
            return Result<bool>.Failure(UserErrors.ValidationFailed("مزود خدمة جوجل غير مفعل."));

        var googleTokenResult = await googleProvider.ValidateTokenAsync(googleIdToken, cancellationToken);
        if (!googleTokenResult.IsSuccess)
            return Result<bool>.Failure(googleTokenResult.Errors);

        var googleUser = googleTokenResult.Data!;

        // 3. 🚨 التأكد أن إيميل جوجل يطابق إيميل الدعوة
        if (!string.Equals(googleUser.Email, invitation.Email, StringComparison.OrdinalIgnoreCase))
        {
            return Result<bool>.Failure(UserErrors.ValidationFailed("البريد الإلكتروني لحساب جوجل لا يطابق البريد الإلكتروني الموجهة له الدعوة."));
        }

        // 4. تقسيم الاسم القادم من جوجل إلى أجزائه
        var nameParts = googleUser.FullName?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];
        var firstName = nameParts.Length > 0 ? nameParts[0] : googleUser.Email;
        var lastName = nameParts.Length > 1 ? nameParts[^1] : string.Empty;
        var middleName = nameParts.Length > 2 ? string.Join(" ", nameParts[1..^1]) : null;

        // 5. إنشاء حساب ApplicationUser (بدون كلمة مرور وربط صورة البروفايل)
        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = invitation.Email,
            Email = invitation.Email,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            AvatarUrl = googleUser.AvatarUrl, // 👈 سحبنا الصورة من جوجل
            EmailConfirmed = true
        };

        // CreateAsync بدون تمرير Password
        var createResult = await userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            var details = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result<bool>.Failure(UserErrors.CreationFailed(details));
        }

        // 6. ربط الحساب بتسجيل دخول جوجل (External Login) باستخدام ProviderUserId
        var loginInfo = new UserLoginInfo("Google", googleUser.ProviderUserId, "Google");
        var addLoginResult = await userManager.AddLoginAsync(user, loginInfo);

        if (!addLoginResult.Succeeded)
        {
            await userManager.DeleteAsync(user); // Rollback
            return Result<bool>.Failure(UserErrors.CreationFailed("فشل ربط الحساب بجوجل."));
        }

        // 7. إضافة الدور (Role)
        var roleResult = await userManager.AddToRoleAsync(user, invitation.Role);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user); // Rollback
            return Result<bool>.Failure(UserErrors.CreationFailed("فشل تعيين الدور الوظيفي للمستخدم."));
        }

        // 8. إنشاء البروفايل الخاص بالدور (Doctor / Receptionist)
        switch (invitation.Role)
        {
            case Roles.Doctor:
                var doctor = new Doctor
                {
                    Id = Guid.CreateVersion7(),
                    ApplicationUserId = user.Id,
                    SpecializationId = invitation.SpecializationId
                        ?? throw new InvalidOperationException("التخصص مطلوب لإنشاء حساب طبيب.")
                };
                context.Add(doctor);
                break;

            case Roles.Receptionist:
                var receptionist = new Receptionist
                {
                    Id = Guid.CreateVersion7(),
                    ApplicationUserId = user.Id,
                    SpecializationId = invitation.SpecializationId
                        ?? throw new InvalidOperationException("التخصص مطلوب لإنشاء حساب وظيفي.")
                };
                context.Add(receptionist);
                break;

            case Roles.Admin:
                break;
        }

        // 9. إغلاق الدعوة
        invitation.IsUsed = true;
        invitation.UsedAtUtc = dateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("تم إكمال قبول الدعوة وتفعيل حساب {Email} بنجاح عبر حساب Google", user.Email);

        return Result<bool>.Success(true);
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToHexString(randomBytes).ToLowerInvariant();
    }
}