using ClinicOS.Api.Controllers.Base;
using ClinicOS.Domain.Constants;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOS.Api.Controllers.Test;

[Route("api/test/admin")]
public class TestAdminController(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : BaseApiController
{
    /// <summary>
    /// إنشاء حساب Admin تجريبي بالبيانات الصحيحة المربوطة بالنظام
    /// </summary>
    [HttpPost("seed-admin")]
    public async Task<IResult> SeedAdmin(
        [FromQuery] string email = "Aymansalama48@yahoo.com",
        [FromQuery] string password = "AdminPassword123!")
    {
        // تنظيف البريد الإلكتروني من أي مسافات أو أحرف مخفية
        email = email.Trim().Replace("ِ", "").Trim();

        // 1. التأكد من وجود دور الأدمن الأساسي
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            var role = new ApplicationRole(Roles.Admin, "System Administrator")
            {
                IsSystemRole = true
            };
            await roleManager.CreateAsync(role);
        }

        // 2. فحص وجود حساب مسجل بنفس الإيميل
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            return Results.BadRequest($"المستخدم صاحب البريد ({email}) موجود بالفعل.");
        }

        // 3. بناء كيان المستخدم التجريبي المخصص لـ ClinicOS
        var adminUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "System",
            LastName = "Admin",
            EmailConfirmed = true,
            IsActive = true
        };

        // 4. حفظ المستخدم وحفظ كلمة المرور المشفرة
        var createResult = await userManager.CreateAsync(adminUser, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Results.BadRequest($"فشل إنشاء حساب الأدمن: {errors}");
        }

        // 5. ربطه بدور Admin
        var roleResult = await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        if (!roleResult.Succeeded)
        {
            return Results.BadRequest("تم إنشاء الحساب ولكن فشل إسناد دور Admin له.");
        }

        return Results.Ok(new
        {
            Message = "تم إنشاء حساب الأدمن التجريبي بنجاح!",
            UserId = adminUser.Id,
            Email = email,
            Password = password,
            FullName = adminUser.FullName
        });
    }
}